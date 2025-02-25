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
        public async Task<IActionResult> CrearEvento([FromBody] EventosAgenda evento)
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
    }
}