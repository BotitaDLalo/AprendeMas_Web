using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class GruposController : ControllerBase
    {
        private readonly DataContext _context;

        public GruposController(DataContext context)
        {
            _context = context;
        }

        public async Task<List<object>> ConsultaGruposMaterias()
        {
            try
            {
                var lsGrupos = await _context.tbGrupos.ToListAsync();


                var listaGruposMaterias = new List<object>();
                foreach (var grupo in lsGrupos)
                {
                    var lsMateriasId = await _context.tbGruposMaterias.Where(a => a.GrupoId == grupo.GrupoId).Select(a => a.MateriaId).ToListAsync();

                    var lsMaterias = await _context.tbMaterias.Where(a => lsMateriasId.Contains(a.MateriaId)).Select(m => new
                    {
                        m.MateriaId,
                        m.NombreMateria,
                        m.Descripcion,
                        //m.CodigoColor,
                        actividades = _context.tbActividades.Where(a => a.MateriaId == m.MateriaId).ToList()
                    }).ToListAsync();


                    listaGruposMaterias.Add(new
                    {
                        grupoId = grupo.GrupoId,
                        nombreGrupo = grupo.NombreGrupo,
                        descripcion = grupo.Descripcion,
                        codigoAcceso = grupo.CodigoAcceso,
                        codigoColor = grupo.CodigoColor,
                        materias = lsMaterias
                    });
                }

                return listaGruposMaterias;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<object>> ConsultaGruposCreados()
        {
            try
            {
                var lsGrupos = await _context.tbGrupos.Select(a => new
                {
                    a.GrupoId,
                    a.NombreGrupo
                }).ToListAsync<object>();

                return lsGrupos;
            }
            catch (Exception)
            {
                return [];
            }
        }

        [HttpGet("ObtenerGruposCreados")]
        public async Task<ActionResult<List<GrupoRegistro>>> ObtenerGruposCreados()
        {
            try
            {
                var lsGrupos = await ConsultaGruposCreados();
                return Ok(lsGrupos);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    e.Message
                });
            }
        }

        [HttpGet("ObtenerGruposMaterias")]
        public async Task<ActionResult<List<GrupoRegistro>>> ObtenerGruposMaterias()
        {
            try
            {
                var listaGruposMaterias = await ConsultaGruposMaterias();

                return Ok(listaGruposMaterias);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    e.Message
                });
            }
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<GrupoRegistro>> GetGroup(int id)
        //{
        //    var group = await _context.Grupos.Include(g => g.Materias).FirstOrDefaultAsync(g => g.GrupoId == id);
        //    if (group is null) return NotFound("Grupo no encontrado");

        //    return Ok(group);
        //}

        [HttpPost("CrearGrupo")]
        public async Task<ActionResult<List<GrupoRegistro>>> CrearGrupo([FromBody] GrupoRegistro group)
        {
            try
            {
                group.CodigoAcceso = "AS4A65S";

                _context.tbGrupos.Add(group);
                await _context.SaveChangesAsync();
                return Ok(await _context.tbGrupos.ToListAsync());
            }
            catch (DbUpdateException ex)
            {
                // Captura la excepción interna para más detalles
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Internal server error: {innerException}");
            }
        }

        [HttpPost("CrearGrupoMaterias")]
        public async Task<ActionResult<List<GrupoRegistro>>> CrearGrupoMaterias([FromBody] GrupoMateriasRegistro group)
        {
            try
            {
                List<MateriaRegistro> lsMaterias = group.Materias.Select(a => new MateriaRegistro { NombreMateria = a.NombreMateria, Descripcion = a.Descripcion }).ToList();


                var nuevoGrupo = new GrupoRegistro
                {
                    NombreGrupo = group.NombreGrupo,
                    Descripcion = group.Descripcion,
                    CodigoColor = group.CodigoColor,
                    CodigoAcceso = "CODIGOPRUEBA"
                };

                _context.tbGrupos.Add(nuevoGrupo);


                _context.tbMaterias.AddRange(lsMaterias);

                await _context.SaveChangesAsync();

                var nuevoGrupoId = nuevoGrupo.GrupoId;
                var lsMateriasId = lsMaterias.Select(a => a.MateriaId).ToList();
                List<GruposMaterias> vinculos = [];
                foreach (var materiaId in lsMateriasId)
                {
                    GruposMaterias vinculo = new()
                    {
                        GrupoId = nuevoGrupoId,
                        MateriaId = materiaId
                    };
                    vinculos.Add(vinculo);
                }
                _context.tbGruposMaterias.AddRange(vinculos);

                await _context.SaveChangesAsync();

                var lsGruposMaterias = await ConsultaGruposMaterias();
                return Ok(lsGruposMaterias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el grupo y materias: {ex.Message}");
            }
        }



        [HttpPut]
        public async Task<ActionResult<List<GrupoRegistro>>> UpdateGroup(GrupoRegistro updatedGroup)
        {
            var dbGroup = await _context.tbGrupos.FindAsync(updatedGroup.GrupoId);
            if (dbGroup is null) return NotFound("Grupo no encontrado");


            dbGroup.NombreGrupo = updatedGroup.NombreGrupo;
            dbGroup.Descripcion = updatedGroup.Descripcion;
            dbGroup.CodigoAcceso = updatedGroup.CodigoAcceso;
            //dbGroup.TipoUsuario = updatedGroup.TipoUsuario;

            await _context.SaveChangesAsync();
            return Ok(await _context.tbGrupos.ToListAsync());
        }

        [HttpDelete]
        public async Task<ActionResult<List<GruposController>>> DeleteGroup(int id)
        {
            var dbGroup = await _context.tbGrupos.FindAsync(id);
            if (dbGroup is null) return NotFound("Grupo no encontrada");

            _context.tbGrupos.Remove(dbGroup);
            await _context.SaveChangesAsync();
            return Ok(await _context.tbGrupos.ToListAsync());
        }
    }
}