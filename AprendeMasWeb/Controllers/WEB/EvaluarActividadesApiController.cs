// Se importa el espacio de nombres para acceder a la base de datos y los controladores
using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
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
        /*
        // Controlador para obtener arrays filtrados por entregados y no entregados
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
                .Select(aa => new
                {
                    aa.AlumnoActividadId,
                    aa.AlumnoId,
                    aa.FechaEntrega,
                    aa.EstatusEntrega
                })
                .ToListAsync();

            if (alumnosActividades == null || !alumnosActividades.Any())
            {
                return NotFound(new { error = $"No se encontraron registros para la actividadId {request.ActividadId}" });
            }

            // Separar en entregados y no entregados
            var noEntregados = alumnosActividades
                .Where(aa => !aa.EstatusEntrega) // Solo los que no han entregado
                .Select(aa => new
                {
                    aa.AlumnoActividadId,
                    aa.FechaEntrega,
                    aa.EstatusEntrega
                })
                .ToList();

            // Obtener los IDs de actividades entregadas
            var entregadosIds = alumnosActividades
                .Where(aa => aa.EstatusEntrega) // Solo los que han entregado
                .Select(aa => aa.AlumnoActividadId)
                .ToList();

            // Obtener la información de los entregados
            var entregados = await _context.tbEntregablesAlumno
                .Where(ea => entregadosIds.Contains(ea.AlumnoActividadId))
                .Select(ea => new
                {
                    ea.EntregaId,
                    ea.AlumnoActividadId,
                    ea.Respuesta
                })
                .ToListAsync();

            // Retornar resultado en formato JSON
            return Ok(new
            {
                NoEntregados = noEntregados,
                Entregados = entregados
            });
        }}*/

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

            var entregadosFormato = entregados
                .Select(ea => new
                {
                    AlumnoActividad = alumnosActividades.FirstOrDefault(aa => aa.AlumnoActividadId == ea.AlumnoActividadId),
                    Entrega = new
                    {
                        ea.EntregaId,
                        ea.AlumnoActividadId,
                        ea.Respuesta
                    }
                })
                .Select(e => new
                {
                    e.AlumnoActividad.AlumnoActividadId,
                    FechaEntrega = e.AlumnoActividad.FechaEntrega,
                    EstatusEntrega = true,
                    e.AlumnoActividad.Alumnos.AlumnoId,
                    e.AlumnoActividad.Alumnos.Nombre,
                    e.AlumnoActividad.Alumnos.ApellidoPaterno,
                    e.AlumnoActividad.Alumnos.ApellidoMaterno,
                    Entrega = e.Entrega
                })
                .ToList();

            // Retornar resultado en formato JSON
            return Ok(new
            {
                NoEntregados = noEntregados,
                Entregados = entregadosFormato
            });
        }



    }

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
