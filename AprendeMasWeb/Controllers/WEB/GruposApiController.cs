using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Route("api/[controller]")]
    [ApiController]
    public class GruposApiController : ControllerBase
    {
        private readonly DataContext _context;
        public GruposApiController(DataContext context)
        {
            _context = context;

        }

        //Aqui se crea el grupo 
        [HttpPost("CrearGrupo")]
        public async Task<IActionResult> CrearGrupo([FromBody] Grupos grupo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos del grupo invalidos.");
            }

            grupo.CodigoAcceso = ObtenerClaveGrupo();
            _context.tbGrupos.Add(grupo);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Grupo creado con exito.", grupoId = grupo.GrupoId });
        }

        //Genera una clave para el grupo
        private string ObtenerClaveGrupo()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 8).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }

        [HttpGet("ObtenerGrupos/{docenteId}")]
        public async Task<IActionResult> ObtenerGrupos(int docenteId)
        {
            var grupos = await _context.tbGrupos
                .Where(g => g.DocenteId == docenteId)
                .ToListAsync();
            return Ok(grupos);
        }
    }
}
