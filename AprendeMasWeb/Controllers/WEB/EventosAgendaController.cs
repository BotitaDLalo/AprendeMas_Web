using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Docente")]
    [Route("api/[controller]")]
    [ApiController]
    public class EventosAgendaController : ControllerBase
    {
        private readonly DataContext _context;

        public EventosAgendaController(DataContext context)
        {
            _context = context;
        }

        // Obtener todos los eventos de un docente
        [HttpGet("docente/{docenteId}")]
        public async Task<IActionResult> GetEventosPorDocente(int docenteId)
        {
            var eventos = await _context.tbEventosAgenda
                .Where(e => e.DocenteId == docenteId)
                .ToListAsync();

            return Ok(eventos);
        }

        // Guardar un nuevo evento
        [HttpPost]
        public async Task<IActionResult> CrearEvento([FromBody] tbEventosAgenda evento)
        {
            if (evento == null)
            {
                return BadRequest("Datos inválidos");
            }

            _context.tbEventosAgenda.Add(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventosPorDocente), new { docenteId = evento.DocenteId }, evento);
        }

        // Actualizar un evento
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarEvento(int id, [FromBody] tbEventosAgenda evento)
        {
            if (id != evento.EventoId)
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
            var evento = await _context.tbEventosAgenda.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }

            _context.tbEventosAgenda.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
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