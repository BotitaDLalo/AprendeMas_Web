using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventosAgendaAlumnoController : ControllerBase
	{
		private readonly DataContext _context;

		public EventosAgendaAlumnoController(DataContext context)
		{
			_context = context;
		}

		// Obtener todos los eventos de un alumno
		[HttpGet("alumno/{alumnoId}")]
		public async Task<IActionResult> GetEventosPorAlumno(int alumnoId)
		{
			var eventos = await _context.tbEventosAgendaAlumno
				.Where(e => e.AlumnoId == alumnoId)
				.ToListAsync();

			return Ok(eventos);
		}

		// Guardar un nuevo evento
		[HttpPost]
		public async Task<IActionResult> CrearEvento([FromBody] EventosAgendaAlumno evento)
		{
			if (evento == null)
			{
				return BadRequest("Datos inválidos");
			}

			_context.tbEventosAgendaAlumno.Add(evento);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetEventosPorAlumno), new { alumnoId = evento.AlumnoId }, evento);
		}

		// Actualizar un evento
		[HttpPut("{id}")]
		public async Task<IActionResult> ActualizarEvento(int id, [FromBody] EventosAgendaAlumno evento)
		{
			if (id != evento.EventoAlumnoId)
			{
				return BadRequest("El ID no coincide");
			}

			_context.Entry(evento).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// Eliminar un evento
		[HttpDelete("{id}")]
		public async Task<IActionResult> EliminarEvento(int id)
		{
			var evento = await _context.tbEventosAgendaAlumno.FindAsync(id);
			if (evento == null)
			{
				return NotFound();
			}

			_context.tbEventosAgendaAlumno.Remove(evento);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
