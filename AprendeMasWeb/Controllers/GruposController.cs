using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
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
        #region Docente
        private static string ObtenerClave()
        {
            int length = 8;

            StringBuilder str_build = new();
            Random random = new();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            return str_build.ToString();
        }

        public async Task<List<object>> ConsultaGruposMaterias(int docenteId)
        {
            try
            {
                var lsGrupos = _context.tbGrupos.Where(a=>a.DocenteId == docenteId).ToList();

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
        public async Task<ActionResult<List<Grupos>>> ObtenerGruposCreados(int docenteId)
        {
            try
            {
                var lsGrupos = await _context.tbGrupos.Where(a=>a.DocenteId == docenteId)
                    .Select(a => new
                    {
                        a.GrupoId,
                        a.NombreGrupo
                    }).ToListAsync();

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

        [HttpGet("ObtenerGruposMateriasDocente")]
        public async Task<ActionResult<List<Grupos>>> ObtenerGruposMateriasDocente(int docenteId)
        {
            try
            {
                var lsGrupos = await _context.tbGrupos.Where(a=>a.DocenteId == docenteId).ToListAsync();


                var listaGruposMaterias = new List<object>();
                foreach (var grupo in lsGrupos)
                {
                    var lsMateriasId = await  _context.tbGruposMaterias.Where(a => a.GrupoId == grupo.GrupoId).Select(a => a.MateriaId).ToListAsync();

                    var lsMaterias = await  _context.tbMaterias.Where(a => lsMateriasId.Contains(a.MateriaId)).Select(m => new
                    {
                        m.MateriaId,
                        m.NombreMateria,
                        m.Descripcion,
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




        [HttpPost("CrearGrupo")]
        public async Task<ActionResult<List<Grupos>>> CrearGrupo([FromBody] Grupos group)
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
        public async Task<ActionResult<List<Grupos>>> CrearGrupoMaterias([FromBody] GrupoMateriasRegistro group)
        {
            try
            {
                int docenteId = group.DocenteId;

                List<Materias> lsMaterias = [];
                foreach (var materia in group.Materias)
                {
                    string codigoAccesoMateria = ObtenerClave();
                    Materias nuevaMateria= new() 
                    { 
                        DocenteId = docenteId,
                        NombreMateria = materia.NombreMateria,
                        Descripcion = materia.Descripcion,
                        CodigoAcceso = codigoAccesoMateria
                    };

                    lsMaterias.Add(nuevaMateria);
                }
                string codigoAccesoGrupo = ObtenerClave();
                Grupos nuevoGrupo = new()
                { 
                    DocenteId = group.DocenteId,
                    NombreGrupo = group.NombreGrupo,
                    Descripcion = group.Descripcion,
                    //CodigoColor = group.CodigoColor,
                    CodigoAcceso = codigoAccesoGrupo
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

                var lsGruposMaterias = await ConsultaGruposMaterias(docenteId);

                return Ok(lsGruposMaterias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el grupo y materias: {ex.Message}");
            }
        }


        [HttpPut("ActualizarGrupo")]
        public async Task<ActionResult<List<Grupos>>> ActualizarGrupo([FromBody]Grupos updatedGroup)
        {
            try
            {
                int grupoId = updatedGroup.GrupoId;
                var dbGroup = await _context.tbGrupos.FindAsync(grupoId);
                if (dbGroup is null) return NotFound("Grupo no encontrado");


                dbGroup.NombreGrupo = updatedGroup.NombreGrupo;
                dbGroup.Descripcion = updatedGroup.Descripcion;
                dbGroup.CodigoColor = updatedGroup.CodigoColor;
                //dbGroup.TipoUsuario = updatedGroup.TipoUsuario;

                await _context.SaveChangesAsync();

                var grupoActualizado = _context.tbGrupos.Where(a=>a.GrupoId==grupoId).FirstOrDefault();
                return Ok(grupoActualizado);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<ActionResult<List<GruposController>>> DeleteGroup(int id)
        {
            var dbGroup = await _context.tbGrupos.FindAsync(id);
            if (dbGroup is null) return NotFound("Grupo no encontrado");

            _context.tbGrupos.Remove(dbGroup);
            await _context.SaveChangesAsync();
            return Ok(await _context.tbGrupos.ToListAsync());
        }
        #endregion


        #region Alumno
        [HttpGet("ObtenerGruposMateriasAlumno")]
        public async Task<ActionResult<List<Grupos>>> ObtenerGruposMateriasAlumno(int alumnoId)
        {
            try
            {
                var lsGruposAlumnosId = await _context.tbAlumnosGrupos.Where(a => a.AlumnoId == alumnoId).Select(a => a.GrupoId).ToListAsync();

                var lsGrupos = await _context.tbGrupos.Where(a => lsGruposAlumnosId.Contains(a.GrupoId)).ToListAsync();

                var listaGruposMaterias = new List<object>();
                foreach (var grupo in lsGrupos)
                {
                    var lsMateriasGrupoId = await _context.tbGruposMaterias.Where(a => a.GrupoId == grupo.GrupoId).Select(a => a.MateriaId).ToListAsync();


                    var lsMaterias = await _context.tbMaterias.Where(a => lsMateriasGrupoId.Contains(a.MateriaId)).Select(m => new
                    {
                        m.MateriaId,
                        m.NombreMateria,
                        m.Descripcion,
                        actividades = _context.tbActividades.Where(a => a.MateriaId == m.MateriaId).ToList()
                    }).ToListAsync();


                    listaGruposMaterias.Add(new
                    {
                        grupoId = grupo.GrupoId,
                        nombreGrupo = grupo.NombreGrupo,
                        descripcion = grupo.Descripcion,
                        //codigoAcceso = grupo.CodigoAcceso,
                        codigoColor = grupo.CodigoColor,
                        materias = lsMaterias
                    });
                }


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


        #endregion
    }
}