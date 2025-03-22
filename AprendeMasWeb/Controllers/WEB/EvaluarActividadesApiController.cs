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

    }
}
