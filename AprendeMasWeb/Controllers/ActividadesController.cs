using AprendeMasWeb.Data;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Models;
using Microsoft.EntityFrameworkCore;
using AprendeMasWeb.Models.DBModels;
using AprendeMasWeb.Services;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITiposActividadesService _tiposActividadesService;

        public ActividadesController(DataContext context, ITiposActividadesService tiposActividadesService)
        {
            _context = context;
            _tiposActividadesService = tiposActividadesService;
        }

        // Cambiar el tipo de retorno a ActionResult<List<object>> para que pueda ser usado en respuestas HTTP
        public async Task<List<object>> ConsultaActividades()
        {
            try
            {
                var materiasActividades = await _context.tbMateriasActividades
                    .Include(ma => ma.Actividades)
                    .Include(ma => ma.Materias)
                    .ToListAsync();

                var listaActividades = materiasActividades
                    .Where(ma => ma.Actividades != null && ma.Materias != null)
                    .Select(ma => new
                    {
                        actividadId = ma.Actividades!.ActividadId,
                        nombreActividad = ma.Actividades!.NombreActividad,
                        descripcionActividad = ma.Actividades!.Descripcion,
                        fechaCreacionActividad = ma.Actividades!.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
                        fechaLimiteActividad = ma.Actividades!.FechaLimite.ToString("yyyy-MM-ddTHH:mm:ss"),
                        tipoActividadId = ma.Actividades!.TipoActividadId,
                        puntaje = ma.Actividades!.Puntaje,
                        materiaId = ma.MateriaId,
                    })
                    .ToList();

                return listaActividades.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                //return BadRequest($"Ocurrió un error al obtener las actividades: {ex.Message}");
                return [];
            }
        }


        // Cambiar el tipo de retorno a ActionResult<List<object>> para ser consistente
        public async Task<ActionResult<List<object>>> ConsultarActividadesCreadas()
        {
            try
            {
                var lsActividades = await _context.tbActividades.Select(a => new
                {
                    a.ActividadId,
                    a.NombreActividad
                }).ToListAsync();

                return Ok(lsActividades); // Retorna la lista de actividades creadas
            }
            catch (Exception)
            {
                return BadRequest("Ocurrió un error al obtener las actividades creadas.");
            }
        }

        public async Task<ActionResult<List<object>>> ConsultaActividadesPorMateria(int materiaId)
        {
            try
            {
                var materiasActividades = await _context.tbMateriasActividades
                    .Include(ma => ma.Actividades)
                    .Include(ma => ma.Materias)
                    .Where(ma => ma.MateriaId == materiaId && ma.Actividades != null && ma.Materias != null)
                    .ToListAsync();

                var listaActividades = materiasActividades
                    .Select(ma => new
                    {
                        actividadId = ma.Actividades!.ActividadId,
                        nombreActividad = ma.Actividades!.NombreActividad,
                        descripcionActividad = ma.Actividades!.Descripcion,
                        fechaCreacionActividad = ma.Actividades!.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
                        fechaLimiteActividad = ma.Actividades!.FechaLimite.ToString("yyyy-MM-ddTHH:mm:ss"),
                        tipoActividadId = ma.Actividades!.TipoActividadId,
                        puntaje = ma.Actividades!.Puntaje,
                        materiaId = ma.MateriaId
                    })
                    .ToList();

                return listaActividades.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al obtener las actividades para la materia {materiaId}: {ex.Message}");
            }
        }

        [HttpGet("ObtenerActividadesPorMateria/{materiaId}")]
        public async Task<ActionResult<List<object>>> ObtenerActividadesPorMateria(int materiaId)
        {

            try
            {
                var lsActividades = await ConsultaActividadesPorMateria(materiaId);
                return lsActividades;
            }
            catch (Exception e) 
            {
                return BadRequest(new { e.Message });
            }

        }



        // El tipo de retorno debe ser ActionResult<List<object>> porque estamos devolviendo una lista de objetos
        [HttpGet("ObtenerActividades")]
        public async Task<ActionResult<List<object>>> ObtenerActividades()
        {
            try
            {
                var lsActividades = await ConsultaActividades();

                return Ok(lsActividades); // Retorna la lista obtenida de ConsultaActividades
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message }); // En caso de error, retornamos el mensaje de la excepción
            }
        }
    


        // Obtener una actividad específica
        [HttpGet("{id}")]
        public async Task<ActionResult<Actividades>> ObtenerActividad(int id)
        {
            var activity = await _context.tbActividades.FindAsync(id);
            if (activity == null) return NotFound("Actividad no encontrada"); // Retorna un mensaje adecuado si no se encuentra la actividad

            return Ok(activity); // Si la actividad se encuentra, la retornamos
        }

        [HttpPost("CrearActividad")]
        public async Task<ActionResult<List<Actividades>>> CrearActividad([FromBody] Actividades nuevaActividad)
        {
            try
            {
                int materiaId = nuevaActividad.MateriaId;
                // Verificar si la materia existe
                var materia = await _context.tbMaterias.FindAsync(materiaId);
                if (materia == null)
                {
                    return BadRequest("La materia asociada no existe.");
                }

                // Asignar el ID de la materia desde la ruta
                nuevaActividad.MateriaId = materiaId;

                // Validar campos no nulos o con valores incorrectos
                if (string.IsNullOrWhiteSpace(nuevaActividad.NombreActividad))
                {
                    return BadRequest("El nombre de la actividad es obligatorio.");
                }

                if (nuevaActividad.FechaLimite == null || nuevaActividad.FechaLimite == default(DateTime))
                {
                    return BadRequest("La fecha límite de la actividad es inválida.");
                }

                // Generar automáticamente la fecha de creación
                nuevaActividad.FechaCreacion = DateTime.Now;

                // Obtener o crear el tipo de actividad si no se especifica
                if (nuevaActividad.TipoActividadId == 0)
                {
                    var tipoActividad = await _tiposActividadesService.GetOrCreateTipoActividad(1); // Por defecto, 1 es "Actividad"
                    nuevaActividad.TipoActividadId = tipoActividad.TipoActividadId;
                }

                // Guardar la actividad en la base de datos
                _context.tbActividades.Add(nuevaActividad);
                await _context.SaveChangesAsync();

                // Crear relación con la materia en la tabla intermedia
                var relacionMateriaActividad = new MateriasActividades
                {
                    MateriaId = materiaId,
                    ActividadId = nuevaActividad.ActividadId
                };

                _context.tbMateriasActividades.Add(relacionMateriaActividad);
                await _context.SaveChangesAsync();

                return Ok(await ConsultaActividades());
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al actualizar la base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

        //public async Task<ActionResult<List<object>>> ConsultaActividadesPorMateria(int materiaId)
        //{
        //    try
        //    {
        //        var materiasActividades = await _context.tbMateriasActividades
        //            .Include(ma => ma.Actividades)
        //            .Include(ma => ma.Materias)
        //            .Where(ma => ma.MateriaId == materiaId && ma.Actividades != null && ma.Materias != null)
        //            .ToListAsync();

        //        var listaActividades = materiasActividades
        //            .Select(ma => new
        //            {
        //                actividadId = ma.Actividades!.ActividadId,
        //                nombreActividad = ma.Actividades!.NombreActividad,
        //                descripcionActividad = ma.Actividades!.Descripcion,
        //                fechaCreacionActividad = ma.Actividades!.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
        //                fechaLimiteActividad = ma.Actividades!.FechaLimite.ToString("yyyy-MM-ddTHH:mm:ss"),
        //                tipoActividadId = ma.Actividades!.TipoActividadId,
        //                materiaId = ma.MateriaId
        //            })
        //            .ToList();

        //        return listaActividades.Cast<object>().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Ocurrió un error al obtener las actividades para la materia {materiaId}: {ex.Message}");
        //    }
        //}

        //[HttpGet("ObtenerActividadesPorMateria/{materiaId}")]
        //public async Task<ActionResult<List<object>>> ObtenerActividadesPorMateria(int materiaId)
        //{
        //    return await ConsultaActividadesPorMateria(materiaId);
        //}




        [HttpPut("ActualizarActividad/{id}")]
        public async Task<ActionResult<Actividades>> UpdateActivity(int id, Actividades updatedActivity)
        {
            var dbActivity = await _context.tbActividades.FindAsync(id);
            if (dbActivity is null) return NotFound("Actividad no encontrada");

            dbActivity.NombreActividad = updatedActivity.NombreActividad;
            dbActivity.Descripcion = updatedActivity.Descripcion;
            dbActivity.FechaLimite = updatedActivity.FechaLimite;

            await _context.SaveChangesAsync();
            return Ok(dbActivity); // Retorna solo la actividad actualizada
        }


        [HttpDelete("EliminarActividad/{id}")]
        public async Task<ActionResult<Actividades>> DeleteActivity(int id)
        {
            try
            {
                // Verificar si la actividad existe
                var dbActivity = await _context.tbActividades
                    .Include(a => a.MateriasActividades) // Incluye las relaciones
                    .FirstOrDefaultAsync(a => a.ActividadId == id);

                if (dbActivity is null) return NotFound("Actividad no encontrada");

                // Eliminar las relaciones dependientes
                _context.tbMateriasActividades.RemoveRange(dbActivity.MateriasActividades);

                // Eliminar la actividad
                _context.tbActividades.Remove(dbActivity);
                await _context.SaveChangesAsync();

                return Ok(await _context.tbActividades.ToListAsync());
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al actualizar la base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }



    }
}