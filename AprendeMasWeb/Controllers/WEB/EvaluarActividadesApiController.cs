// Se importa el espacio de nombres para acceder a la base de datos y los controladores
using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Docente")]
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluarActividadesApiController : ControllerBase
    {
        // Se crea una variable privada para almacenar el contexto de la base de datos
        private readonly DataContext _context;

        // Constructor que recibe el contexto de la base de datos
        public EvaluarActividadesApiController(DataContext context)
        {
            _context = context; // Se asigna el contexto a la variable _context
        }

        //Controlador para obtener los datos de una actividad
        [HttpGet("ObtenerActividadPorId/{actividadId}")]
        public async Task<IActionResult> ObtenerActividadPorId(int actividadId)
        {
            try
            {
                var actividad = await _context.tbActividades
                .Where(a => a.ActividadId == actividadId)
                .Select(a => new
                {
                    a.ActividadId,
                    a.NombreActividad,
                    a.Descripcion,
                    a.FechaCreacion,
                    a.FechaLimite,
                    a.Puntaje
                })
                .FirstOrDefaultAsync();

                if (actividad == null)
                {
                    return NotFound(new { mensaje = "No se encontró la actividad con el ID especificado." });
                }

                return Ok(actividad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la actividad", error = ex.Message });
            }
        }

        // Método para obtener la lista de alumnos que están dentro de la materia > se guardan en array para despues comparar.-HAcer busqueda mas eficiente
        [HttpGet("AlumnosParaCalificarActividades/{materiaId}")]
        public async Task<IActionResult> ObtenerAlumnosPorMateria(int materiaId)
        {
            var alumnos = await _context.tbAlumnosMaterias
                .Where(am => am.MateriaId == materiaId)
                .Join(_context.tbAlumnos,
                    am => am.AlumnoId,
                    a => a.AlumnoId,
                    (am, a) => new
                    {
                        a.AlumnoId,
                        a.Nombre,
                        a.ApellidoPaterno,
                        a.ApellidoMaterno
                    })
                .OrderBy(a => a.ApellidoPaterno) // Ordena por apellido paterno
                .ThenBy(a => a.ApellidoMaterno)  // Si hay empate en el apellido paterno, ordena por el apellido materno
                .ThenBy(a => a.Nombre)           // Si hay empate en el apellido materno, ordena por el nombre
                .ToListAsync();
            return Ok(alumnos);
        }


        [HttpPost("ObtenerActividadesParaEvaluar")]
        public async Task<IActionResult> ObtenerActividadesParaEvaluar([FromBody] EvaluacionRequest request)
        {
            if (request == null || request.Alumnos == null || !request.Alumnos.Any() || request.ActividadId <= 0)
            {
                return BadRequest(new { error = "Datos inválidos en la solicitud" });
            }

            // Extraer los ID de los alumnos
            var alumnoIds = request.Alumnos.Select(a => a.AlumnoId).ToList();

            // Obtener actividades de los alumnos para la actividad específica
            var alumnosActividades = await _context.tbAlumnosActividades
                .Where(aa => alumnoIds.Contains(aa.AlumnoId) && aa.ActividadId == request.ActividadId)
                .Include(aa => aa.Alumnos) // Incluir datos del alumno directamente
                .ToListAsync(); // Obtener datos en memoria para trabajar con LINQ en C#

            if (!alumnosActividades.Any())
            {
                return NotFound(new { error = $"No se encontraron registros para la actividadId {request.ActividadId}" });
            }

            // Separar en no entregados
            var noEntregados = alumnosActividades
                .Where(aa => !aa.EstatusEntrega) // Filtrar en memoria para evitar problemas con EF Core
                .Select(aa => new
                {
                    aa.AlumnoActividadId,
                    aa.Alumnos.AlumnoId,
                    aa.Alumnos.Nombre,
                    aa.Alumnos.ApellidoPaterno,
                    aa.Alumnos.ApellidoMaterno
                })
                .ToList();

            // Obtener entregas con datos de alumnos
            var entregadosIds = alumnosActividades
                .Where(aa => aa.EstatusEntrega)
                .Select(aa => aa.AlumnoActividadId)
                .ToList();

            var entregados = await _context.tbEntregablesAlumno
                .Where(ea => entregadosIds.Contains(ea.AlumnoActividadId))
                .ToListAsync();

            // Obtener los IDs de Entrega para buscar en Calificaciones
            var entregaIds = entregados.Select(e => e.EntregaId).ToList();

            // Buscar calificaciones relacionadas con esas entregas
            var calificaciones = await _context.tbCalificaciones
                .Where(c => entregaIds.Contains(c.EntregaId))
                .ToListAsync();

            // Combinar entregas con calificaciones (si existen)
            var entregadosFormato = entregados
                .Select(ea =>
                {
                    var alumnoActividad = alumnosActividades.FirstOrDefault(aa => aa.AlumnoActividadId == ea.AlumnoActividadId);
                    var calificacion = calificaciones.FirstOrDefault(c => c.EntregaId == ea.EntregaId);

                    return new
                    {
                        alumnoActividad.AlumnoActividadId,
                        FechaEntrega = alumnoActividad.FechaEntrega,
                        EstatusEntrega = true,
                        alumnoActividad.Alumnos.AlumnoId,
                        alumnoActividad.Alumnos.Nombre,
                        alumnoActividad.Alumnos.ApellidoPaterno,
                        alumnoActividad.Alumnos.ApellidoMaterno,
                        Entrega = new
                        {
                            ea.EntregaId,
                            ea.AlumnoActividadId,
                            ea.Respuesta
                        },
                        Calificacion = calificacion != null ? new
                        {
                            calificacion.CalificacionId,
                            calificacion.EntregaId,
                            calificacion.FechaCalificacionAsignada,
                            calificacion.Comentarios,
                            calificacion.Calificacion
                        } : null
                    };
                })
                .ToList();


            // Retornar resultado en formato JSON
            return Ok(new
            {
                NoEntregados = noEntregados,
                Entregados = entregadosFormato
            });
        }

        //Si un alumno es agregado a la materia 
        [HttpPost("AsignarActividadesPendientes")]
        public async Task<IActionResult> AsignarActividadesPendientes([FromBody] int alumnoId)
        {
            try
            {
                // Verificar si el alumno existe
                var alumnoExiste = await _context.tbAlumnos.AnyAsync(a => a.AlumnoId == alumnoId);
                if (!alumnoExiste)
                {
                    return BadRequest(new { mensaje = "El alumno no existe." });
                }

                // Obtener la materia del alumno
                var materiasAlumno = await _context.tbAlumnosMaterias
                    .Where(am => am.AlumnoId == alumnoId)
                    .Select(am => am.MateriaId)
                    .ToListAsync();

                if (!materiasAlumno.Any())
                {
                    return BadRequest(new { mensaje = "El alumno no está inscrito en ninguna materia." });
                }

                // Buscar actividades que no tiene asignadas en esas materias
                var actividadesPendientes = await _context.tbActividades
                    .Where(a => materiasAlumno.Contains(a.MateriaId) &&
                                !_context.tbAlumnosActividades.Any(aa => aa.AlumnoId == alumnoId && aa.ActividadId == a.ActividadId))
                    .ToListAsync();

                // Asignar cada actividad pendiente al alumno
                foreach (var actividad in actividadesPendientes)
                {
                    var nuevaRelacion = new tbAlumnosActividades
                    {
                        ActividadId = actividad.ActividadId,
                        AlumnoId = alumnoId,
                        FechaEntrega = DateTime.Now, // Se actualiza cuando entregue
                        EstatusEntrega = false
                    };

                    _context.tbAlumnosActividades.Add(nuevaRelacion);
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Actividades asignadas al nuevo alumno." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al asignar actividades.", error = ex.Message });
            }
        }

        // Controlador para registrar o actualizar una calificación
        [HttpPost("RegistrarCalificacion")]
        public async Task<IActionResult> RegistrarCalificacion([FromBody] CalificacionDto calificacionDto)
        {
            if (calificacionDto == null)
            {
                return BadRequest(new { mensaje = "Datos inválidos." });
            }

            // Verificar si la entrega existe en la tabla correcta
            var entregaExiste = await _context.tbEntregablesAlumno.AnyAsync(e => e.EntregaId == calificacionDto.EntregaId);
            if (!entregaExiste)
            {
                return BadRequest(new { mensaje = "La entrega especificada no existe." });
            }

            try
            {
                // Buscar si ya existe una calificación para esta entrega
                var calificacionExistente = await _context.tbCalificaciones
                    .FirstOrDefaultAsync(c => c.EntregaId == calificacionDto.EntregaId);

                if (calificacionExistente != null)
                {
                    // Si ya existe, actualizar los datos
                    calificacionExistente.Calificacion = calificacionDto.Calificacion;
                    calificacionExistente.Comentarios = calificacionDto.Comentario;
                    calificacionExistente.FechaCalificacionAsignada = DateTime.Now;

                    _context.tbCalificaciones.Update(calificacionExistente);
                }
                else
                {
                    // Si no existe, crear una nueva calificación
                    var nuevaCalificacion = new tbCalificaciones
                    {
                        EntregaId = calificacionDto.EntregaId,
                        FechaCalificacionAsignada = DateTime.Now,
                        Comentarios = calificacionDto.Comentario,
                        Calificacion = calificacionDto.Calificacion
                    };

                    _context.tbCalificaciones.Add(nuevaCalificacion);
                }

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Calificación guardada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar la calificación.", error = ex.Message });
            }
        }




    }

}

public class CalificacionDto
{
    public int EntregaId { get; set; }
    public int Calificacion { get; set; }
    public string? Comentario { get; set; }
}



// Modelo de request para recibir los datos
public class EvaluacionRequest
{
    public List<AlumnoDTO> Alumnos { get; set; }
    public int ActividadId { get; set; }
}

public class AlumnoDTO
{
    public int AlumnoId { get; set; }
    public string Nombre { get; set; }
    public string ApellidoPaterno { get; set; }
    public string ApellidoMaterno { get; set; }
}
