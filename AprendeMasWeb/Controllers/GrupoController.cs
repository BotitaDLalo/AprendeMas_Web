using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class GrupoController : ControllerBase
    {
        private readonly DataContext _context;

        public GrupoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<GrupoRegistro>>> GetAllGroups()
        {
            var groups = await _context.Grupos.Include(g => g.Materias).ToListAsync();

            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GrupoRegistro>> GetGroup(int id)
        {
            var group = await _context.Grupos.Include(g => g.Materias).FirstOrDefaultAsync(g => g.GrupoId == id);
            if (group is null) return NotFound("Grupo no encontrado");

            return Ok(group);
        }

        [HttpPost]
        public async Task<ActionResult<List<GrupoRegistro>>> AddGroup(GrupoRegistro group)
        {
            try
            {
                _context.Grupos.Add(group);
                await _context.SaveChangesAsync();
                return Ok(await _context.Grupos.ToListAsync());
            }
            catch (DbUpdateException ex)
            {
                // Captura la excepción interna para más detalles
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Internal server error: {innerException}");
            }
        }



        [HttpPut]
        public async Task<ActionResult<List<GrupoRegistro>>> UpdateGroup(GrupoRegistro updatedGroup)
        {
            var dbGroup = await _context.Grupos.FindAsync(updatedGroup.GrupoId);
            if (dbGroup is null) return NotFound("Grupo no encontrado");


            dbGroup.NombreGrupo = updatedGroup.NombreGrupo;
            dbGroup.Descripcion = updatedGroup.Descripcion;
            dbGroup.CodigoAcceso = updatedGroup.CodigoAcceso;
            dbGroup.TipoUsuario = updatedGroup.TipoUsuario;

            await _context.SaveChangesAsync();
            return Ok(await _context.Grupos.ToListAsync());
        }

        [HttpDelete]
        public async Task<ActionResult<List<GrupoController>>> DeleteGroup(int id)
        {
            var dbGroup = await _context.Grupos.FindAsync(id);
            if (dbGroup is null) return NotFound("Grupo no encontrada");

            _context.Grupos.Remove(dbGroup);
            await _context.SaveChangesAsync();
            return Ok(await _context.Grupos.ToListAsync());
        }
    }
}