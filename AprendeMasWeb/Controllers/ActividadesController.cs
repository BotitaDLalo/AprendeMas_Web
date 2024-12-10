using AprendeMasWeb.Data;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Models;
using Microsoft.EntityFrameworkCore;
using AprendeMasWeb.Models.DBModels;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadesController : ControllerBase
    {
        private readonly DataContext _context;

        public ActividadesController(DataContext context)
        {
            _context = context;
        }

        // Cambiar el tipo de retorno a ActionResult<List<object>> para que pueda ser usado en respuestas HTTP
        public async Task<ActionResult<List<object>>> ConsultaActividades()
        {
            try
            {
                // Obtenemos todas las actividades con su MateriaId
                var lsActividades = await _context.tbActividades
                    .Include(a => a.MateriaId)  // Esto incluye la materia relacionada por MateriaId
                    .ToListAsync();

                var listaActividades = new List<object>();

                foreach (var actividad in lsActividades)
                {
                    listaActividades.Add(new
                    {
                        actividadId = actividad.ActividadId,
                        nombreActividad = actividad.NombreActividad,
                        descripcionActividad = actividad.Descripcion,
                        fechaCreacionActividad = actividad.FechaCreacion,
                        fechaLimiteActividad = actividad.FechaLimite,
                        tipoActividad = actividad.TipoActividadId
                    });
                }

                return listaActividades; // Retornamos la lista de actividades procesadas
            }
            catch (Exception)
            {
                return BadRequest("Ocurrió un error al obtener las actividades."); // En lugar de retornar [], retornamos un BadRequest
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
        public async Task<ActionResult<Actividades>> CrearActividad([FromBody] Actividades nuevaActividad)
        {
            // Verificar si la materia existe
            var materia = await _context.tbMaterias.FindAsync(nuevaActividad.MateriaId);

            if (materia == null)
            {
                return BadRequest("La materia asociada no existe.");
            }

            // Generar automáticamente la fecha de creación
            nuevaActividad.FechaCreacion = DateTime.UtcNow;

            // Proceder con la creación de la actividad
            _context.tbActividades.Add(nuevaActividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerActividad), new { id = nuevaActividad.ActividadId }, nuevaActividad);
        }



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
        public async Task<ActionResult<List<Actividades>>> DeleteActivity(int id)
        {
            var dbActivity = await _context.tbActividades.FindAsync(id);
            if (dbActivity is null) return NotFound("Actividad no encontrada");

            _context.tbActividades.Remove(dbActivity);
            await _context.SaveChangesAsync();
            return Ok(await _context.tbActividades.ToListAsync());
        }


    }
}