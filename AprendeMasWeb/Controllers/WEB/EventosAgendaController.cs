using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AprendeMasWeb.Controllers.WEB
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosAgendaController : ControllerBase
    {
        private readonly DataContext _context;

        public EventosAgendaController(DataContext context)
        {
            _context = context;
        }

        // POST: api/EventosAgenda
        [HttpPost]
        public async Task<IActionResult> CrearEvento([FromBody] tbEventosAgenda evento)
        {
            if (evento == null)
            {
                return BadRequest("Datos inválidos.");
            }

            if (evento.Color != "azul" && evento.Color != "gris")
            {
                return BadRequest("Solo se permiten los colores azul y gris.");
            }

            _context.tbEventosAgenda.Add(evento);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Evento guardado exitosamente" });
        }
		[HttpGet("fecha/{fecha}")]
		public async Task<IActionResult> ObtenerEventosPorFecha(string fecha)
		{
			if (!DateTime.TryParse(fecha, out DateTime fechaSeleccionada))
			{
				return BadRequest(new { mensaje = "Fecha inválida." });
			}

			var eventos = await _context.tbEventosAgenda
				.Where(e => e.FechaInicio.Date == fechaSeleccionada.Date)
				.Select(e => new
				{
					eventoId = e.EventoId, 
					titulo = e.Titulo,
					descripcion = e.Descripcion,
					fechaInicio = e.FechaInicio,
					fechaFinal = e.FechaFinal,
					color = e.Color
				})
				.ToListAsync();

			if (!eventos.Any())
			{
				return Ok(new { mensaje = "No hay eventos para esta fecha." });
			}

			return Ok(eventos);
		}

	}

}