using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;

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

        [HttpGet("ObtenerGrupo")]
        public async Task<ActionResult<List<GrupoRegistro>>> ObtenerGrupos()
        {
            //var groups = await _context.Grupos.Include(g => g.Materias).ToListAsync();

            var lsGrupos = await _context.Grupos.ToListAsync();


            var listaGruposMaterias = new List<object>();
            foreach (var grupo in lsGrupos)
            {
                var lsMateriasId = await _context.GruposMaterias.Where(a => a.GrupoId == grupo.GrupoId).Select(a => a.MateriaId).ToListAsync();

                var lsMaterias = await _context.Materias.Where(a => lsMateriasId.Contains(a.MateriaId)).Select( m => new
                {
                    m.MateriaId,
                    m.NombreMateria,
                    m.Descripcion,
                    m.CodigoColor,
                    actividades = _context.Actividades.Where(a=>a.MateriaId == m.MateriaId).ToList()
                }).ToListAsync();


                listaGruposMaterias.Add(new
                {
                    grupoId = grupo.GrupoId,
                    nombreGrupo = grupo.NombreGrupo,
                    descripcion = grupo.Descripcion,
                    codigoAcceso = grupo.CodigoAcceso,
                    //tipoUsuario = grupo.TipoUsuario,
                    codigoColor = grupo.CodigoColor,
                    materias = lsMaterias
                });
            }

            return Ok(listaGruposMaterias);
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<GrupoRegistro>> GetGroup(int id)
        //{
        //    var group = await _context.Grupos.Include(g => g.Materias).FirstOrDefaultAsync(g => g.GrupoId == id);
        //    if (group is null) return NotFound("Grupo no encontrado");

        //    return Ok(group);
        //}

        [HttpPost("CrearGrupo")]
        public async Task<ActionResult<List<GrupoRegistro>>> CrearGrupo([FromBody]GrupoRegistro group)
        {
            try
            {
                group.CodigoAcceso = "AS4A65S";

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
            //dbGroup.TipoUsuario = updatedGroup.TipoUsuario;

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