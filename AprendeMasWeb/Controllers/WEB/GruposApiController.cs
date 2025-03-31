// Se importan los espacios de nombres necesarios para trabajar con la base de datos y las API de ASP.NET Core
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AprendeMasWeb.Controllers.WEB
{
    // Se define la ruta base para este controlador API (utilizando la convención de API REST)
    [Route("api/[controller]")]
    // Indica que este controlador es para una API
    [ApiController]
    public class GruposApiController : ControllerBase
    {
        // Se declara el contexto de la base de datos para interactuar con los datos de la aplicación
        private readonly DataContext _context;

        // Constructor que recibe el contexto de datos para poder interactuar con la base de datos
        public GruposApiController(DataContext context)
        {
            _context = context; // Asigna el contexto de datos a la variable de la clase
        }

        // Acción para crear un grupo mediante una solicitud POST (API)
        [HttpPost("CrearGrupo")]
        public async Task<IActionResult> CrearGrupo([FromBody] tbGrupos grupo)
        {
            // Verifica si el modelo enviado es válido (ejemplo: los datos del grupo están completos)
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, devuelve un mensaje de error con un estado BadRequest
                return BadRequest("Datos del grupo invalidos.");
            }

            // Genera un código de acceso para el grupo
            grupo.CodigoAcceso = ObtenerClaveGrupo();
            // Agrega el grupo a la base de datos
            _context.tbGrupos.Add(grupo);
            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();
            // Retorna una respuesta exitosa con un mensaje y el ID del grupo creado
            return Ok(new { mensaje = "Grupo creado con exito.", grupoId = grupo.GrupoId });
        }

        // Método privado que genera una clave aleatoria de 8 caracteres para el grupo
        private string ObtenerClaveGrupo()
        {
            var random = new Random(); // Crea una instancia de la clase Random
            // Genera una cadena de 8 caracteres aleatorios entre A y Z
            return new string(Enumerable.Range(0, 8).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }

        // Acción para obtener los grupos pertenecientes a un docente específico
        [HttpGet("ObtenerGrupos/{docenteId}")]
        public async Task<IActionResult> ObtenerGrupos(int docenteId)
        {
            // Consulta los grupos en la base de datos que pertenecen al docente con el ID proporcionado
            var grupos = await _context.tbGrupos
                .Where(g => g.DocenteId == docenteId)
                .ToListAsync();
            // Devuelve una respuesta exitosa con los grupos obtenidos
            return Ok(grupos);
        }

        // Acción POST para asociar materias a un grupo
        [HttpPost("AsociarMaterias")]
        public async Task<IActionResult> AsociarMaterias([FromBody] AsociarMateriasRequest request)
        {
            // Verifica que los datos de la solicitud no sean nulos y que contengan al menos una materia
            if (request == null || request.MateriaIds == null || request.MateriaIds.Count == 0)
            {
                return BadRequest("Datos Invalidos"); // Retorna un error si los datos son inválidos
            }

            // Verifica si el grupo existe en la base de datos
            var grupo = await _context.tbGrupos.FindAsync(request.GrupoId);
            if (grupo == null)
            {
                return NotFound("Grupo no encontrado."); // Si no se encuentra el grupo, retorna un error
            }

            // Elimina las asociaciones previas entre el grupo y las materias (opcional, si quieres reemplazar en lugar de agregar nuevas)
            var asociacionesActuales = _context.tbGruposMaterias.Where(gm => gm.GrupoId == request.GrupoId);
            _context.tbGruposMaterias.RemoveRange(asociacionesActuales);

            // Asocia las materias seleccionadas al grupo
            foreach (var materiaId in request.MateriaIds)
            {
                // Verifica si la materia existe en la base de datos
                var materiaExiste = await _context.tbMaterias.AnyAsync(m => m.MateriaId == materiaId);
                if (materiaExiste)
                {
                    // Crea una nueva relación entre el grupo y la materia
                    var nuevaRelacion = new tbGruposMaterias
                    {
                        GrupoId = request.GrupoId,
                        MateriaId = materiaId
                    };
                    // Agrega la nueva relación a la base de datos
                    _context.tbGruposMaterias.Add(nuevaRelacion);
                }
            }

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();
            // Retorna una respuesta exitosa con un mensaje
            return Ok(new { mensaje = "Materias asociadas correctamente." });
        }
        

        [HttpGet("ObtenerMateriasPorGrupo/{grupoId}")]
        public async Task<IActionResult> ObtenerMateriasPorGrupo(int grupoId)
        {
            // Obtener los IDs de las materias que pertenecen al grupo
            var materiasIds = await _context.tbGruposMaterias
                .Where(gm => gm.GrupoId == grupoId)
                .Select(gm => gm.MateriaId)
                .ToListAsync();

            // Obtener las materias con sus actividades recientes
            var materiasConActividades = await _context.tbMaterias
                .Where(m => materiasIds.Contains(m.MateriaId))
                .Select(m => new
                {
                    m.MateriaId,
                    m.NombreMateria,
                    m.Descripcion,
                    m.CodigoColor,
                    ActividadesRecientes = _context.tbActividades
                        .Where(a => a.MateriaId == m.MateriaId)
                        .OrderByDescending(a => a.FechaCreacion)
                        .Take(2)
                        .Select(a => new
                        {
                            a.ActividadId,
                            a.NombreActividad,
                            a.FechaCreacion
                        })
                        .ToList() // Obtener las actividades recientes
                })
                .ToListAsync();

            return Ok(materiasConActividades);
        }

        //Elimina solo el grupo y deja las materias en apartado materias sin grupo
        [HttpDelete("EliminarGrupo/{grupoId}")]
        public async Task<IActionResult> EliminarGrupo(int grupoId)
        {
            //buscar si el grupo existe en la base de datos
            var grupo = await _context.tbGrupos.FindAsync(grupoId);
            if(grupo == null)
            {
                return NotFound(new { mensaje = "El grupo no existe." });
            }

            //Eliminar todas las relaciones del grupo en la tabla GruposYMaterias
            var relaciones = _context.tbGruposMaterias
                .Where(gm => gm.GrupoId == grupoId);
            _context.tbGruposMaterias.RemoveRange(relaciones);

            //Eliminar el grupo despues de haber eliminado las relaciones 
            _context.tbGrupos.Remove(grupo);

            //Guardamos los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Grupo eliminado correctamente." });
        }

        // Elimina un grupo junto con sus materias, actividades y avisos asociados
        [HttpDelete("EliminarGrupoConMaterias/{grupoId}")]
        public async Task<IActionResult> EliminarGrupoConMaterias(int grupoId)
        {
            // Buscar el grupo en la base de datos
            var grupo = await _context.tbGrupos.FindAsync(grupoId);
            if (grupo == null)
            {
                return NotFound(new { mensaje = "El grupo no existe" });
            }

            // Obtener las relaciones del grupo con las materias
            var relacionesGruposMaterias = _context.tbGruposMaterias.Where(mg => mg.GrupoId == grupoId);
            var materiasIds = relacionesGruposMaterias.Select(r => r.MateriaId).ToList();

            // Obtener actividades asociadas a las materias
            var actividades = _context.tbActividades.Where(a => materiasIds.Contains(a.MateriaId)).ToList();
            var actividadesIds = actividades.Select(a => a.ActividadId).ToList();

            foreach (var actividadId in actividadesIds)
            {
                // Buscar registros en la tabla alumnosActividades relacionados con la actividad
                var alumnosActividades = _context.tbAlumnosActividades.Where(aa => aa.ActividadId == actividadId).ToList();

                foreach (var alumnoActividad in alumnosActividades)
                {
                    int alumnoActividadId = alumnoActividad.AlumnoActividadId;

                    // Buscar entregables asociados
                    var entregables = _context.tbEntregablesAlumno.Where(e => e.AlumnoActividadId == alumnoActividadId).ToList();
                    var entregasIds = entregables.Select(e => e.EntregaId).ToList();

                    // Buscar calificaciones asociadas
                    var calificaciones = _context.tbCalificaciones.Where(c => entregasIds.Contains(c.EntregaId)).ToList();
                    _context.tbCalificaciones.RemoveRange(calificaciones);

                    // Eliminar entregables
                    _context.tbEntregablesAlumno.RemoveRange(entregables);
                }

                // Eliminar alumnosActividades
                _context.tbAlumnosActividades.RemoveRange(alumnosActividades);
            }

            // Eliminar actividades
            _context.tbActividades.RemoveRange(actividades);

            // Obtener y eliminar los avisos asociados a las materias
            var avisos = _context.tbAvisos.Where(av => av.MateriaId.HasValue && materiasIds.Contains(av.MateriaId.Value));
            _context.tbAvisos.RemoveRange(avisos);

            // Eliminar relaciones entre alumnos y materias
            var relacionesAlumnosMaterias = _context.tbAlumnosMaterias.Where(am => materiasIds.Contains(am.MateriaId));
            _context.tbAlumnosMaterias.RemoveRange(relacionesAlumnosMaterias);

            // Eliminar relaciones entre grupos y materias
            _context.tbGruposMaterias.RemoveRange(relacionesGruposMaterias);

            // Obtener y eliminar las materias
            var materias = _context.tbMaterias.Where(m => materiasIds.Contains(m.MateriaId));
            _context.tbMaterias.RemoveRange(materias);

            // Eliminar el grupo
            _context.tbGrupos.Remove(grupo);

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Grupo, materias, actividades y registros relacionados eliminados correctamente" });
        }


        // Método para obtener los detalles de un grupo específica por ID
        [HttpGet("ObtenerGrupo/{id}")]
        public async Task<IActionResult> ObtenerMateria(int id)
        {
            var grupo = await _context.tbGrupos.FindAsync(id);

            if (grupo == null)
            {
                return NotFound("El grupo no existe.");
            }

            return Ok(grupo);  // Devuelve el grupo encontrado
        }


        //Actualizar Grupo
        [HttpPut("ActualizarGrupo")]
        public async Task<IActionResult> EditarAviso([FromBody] tbGrupos model)
        {
            try
            {
                var grupo = await _context.tbGrupos.FindAsync(model.GrupoId);
                if (grupo == null)
                    return NotFound(new { mensaje = "Grupo no encontrado" });

                grupo.NombreGrupo = model.NombreGrupo;
                grupo.Descripcion = model.Descripcion;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Grupo actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el grupo", error = ex.Message });
            }
        }



    }
}

// Modelo para recibir la solicitud desde el frontend (para asociar materias a un grupo)
public class AsociarMateriasRequest
{
    public int GrupoId { get; set; } // ID del grupo
    public List<int> MateriaIds { get; set; } // Lista de IDs de las materias que se asociarán al grupo
}
