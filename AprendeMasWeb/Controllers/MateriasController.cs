using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriasController : Controller
    {
        private readonly DataContext _context;

        public MateriasController(DataContext context)
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


        public async Task<List<object>> ConsultaGrupos()
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


        public async Task<List<tbMaterias>> ConsultaMaterias()
        {
            try
            {
                var lsGruposMaterias = await _context.tbGruposMaterias.Select(a => a.MateriaId).ToListAsync();

                var lsMateriasSinGrupo = await _context.tbMaterias.Where(a => !lsGruposMaterias.Contains(a.MateriaId)).ToListAsync();

                return lsMateriasSinGrupo;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<tbMaterias>> ConsultaMateriasPorDocente(int docenteId)
        {
            try
            {
                var lsGruposMaterias = await _context.tbGruposMaterias.Select(a => a.MateriaId).ToListAsync();

                var lsMateriasSinGrupo = await _context.tbMaterias
                    .Where(a => a.DocenteId == docenteId && !lsGruposMaterias.Contains(a.MateriaId))
                    .ToListAsync();

                return lsMateriasSinGrupo;
            }
            catch (Exception)
            {
                return [];
            }
        }

        [HttpGet("ObtenerMateriasDocente")]
        public async Task<ActionResult<List<tbMaterias>>> ObtenerMateriasSinGrupoDocente(int docenteId)
        {
            try
            {
                List<int> lsMateriasId = await _context.tbMaterias.Where(a => a.DocenteId == docenteId).Select(a => a.MateriaId).ToListAsync();

                List<int> lsGruposMateriasId = await _context.tbGruposMaterias.Where(a => lsMateriasId.Contains(a.MateriaId)).Select(a=>a.MateriaId).ToListAsync();

                lsMateriasId = lsMateriasId.Where(a=>!lsGruposMateriasId.Contains(a)).ToList();

                var lsMaterias = await _context.tbMaterias.Where(a => lsMateriasId.Contains(a.MateriaId)).Select(a => new
                {
                    a.MateriaId,
                    a.NombreMateria,
                    actividades = _context.tbActividades.Where(b=>b.MateriaId == a.MateriaId).ToList()
                }).ToListAsync();

                return Ok(lsMaterias);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    e.Message
                });
            }
        }


        [HttpGet("ObtenerMaterias")]
        public async Task<ActionResult<List<tbMaterias>>> ObtenerMaterias()
        {
            try
            {
                return await ConsultaMaterias();
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error en ObtenerMaterias"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<tbMaterias>> ObtenerMateriaUnica(int id)
        {
            var subject = await _context.tbMaterias.FindAsync(id);
            if (subject is null) return NotFound("Materia no encontrado");

            return Ok(subject);
        }


        [HttpPost("CrearMateriaSinGrupo")]
        public async Task<ActionResult> CrearMateriaSinGrupo([FromBody] tbMaterias materia)
        {
            try
            {
                materia.CodigoAcceso = ObtenerClave();
                _context.tbMaterias.Add(materia);
                await _context.SaveChangesAsync();
                return Ok(await ConsultaMaterias());
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "No se registro la materia" });
            }
        }

        [HttpPost("CrearMateriaGrupos")]
        public async Task<ActionResult<List<MateriaConGrupo>>> CrearMateriaGrupos([FromBody] MateriaConGrupo materiaConGrupo)
        {
            try
            {
                int docenteId = materiaConGrupo.DocenteId;
                var lsGruposId = _context.tbGrupos.Where(a => a.DocenteId == docenteId).Select(a => a.GrupoId).ToList();
                List<int> gruposVinculados = materiaConGrupo.Grupos;
                if (gruposVinculados.All(a => lsGruposId.Contains(a)))
                {

                    tbMaterias materia = new()
                    {
                        DocenteId = docenteId,
                        NombreMateria = materiaConGrupo.NombreMateria,
                        Descripcion = materiaConGrupo.Descripcion,
                        CodigoAcceso = ObtenerClave()
                        //CodigoColor = materiaG.CodigoColor,
                    };


                    _context.tbMaterias.Add(materia);
                    await _context.SaveChangesAsync();



                    var idMateria = materia.MateriaId;


                    foreach (var grupo in gruposVinculados)
                    {

                        tbGruposMaterias gruposMaterias = new()
                        {
                            GrupoId = grupo,
                            MateriaId = idMateria
                        };

                        _context.tbGruposMaterias.Add(gruposMaterias);

                    }
                    await _context.SaveChangesAsync();

                    var lsGruposMaterias = await ConsultaGrupos();

                    return Ok(lsGruposMaterias);
                }
                else
                {
                    return BadRequest(new { mensaje = "Un grupo no pertenece al docente" });
                }
            }
            catch (DbUpdateException ex)
            {
                // Captura la excepción interna para más detalles
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Internal server error: {innerException}");
            }
        }


        [HttpPut]
        public async Task<ActionResult<List<tbMaterias>>> UpdateSubject(tbMaterias updatedSubject)
        {
            var dbSubject = await _context.tbMaterias.FindAsync(updatedSubject.MateriaId);
            if (dbSubject is null) return NotFound("Materia no encontrado");


            dbSubject.NombreMateria = updatedSubject.NombreMateria;
            dbSubject.Descripcion = updatedSubject.Descripcion;

            await _context.SaveChangesAsync();
            return Ok(await _context.tbMaterias.ToListAsync());
        }

        [HttpDelete]
        public async Task<ActionResult<List<MateriasController>>> DeleteSubject(int id)
        {
            var dbSubject = await _context.tbMaterias.FindAsync(id);
            if (dbSubject is null) return NotFound("Materia no encontrada");

            _context.tbMaterias.Remove(dbSubject);
            await _context.SaveChangesAsync();
            return Ok(await _context.tbMaterias.ToListAsync());
        }
        #endregion

        #region Alumno

        [HttpGet("ObtenerMateriasAlumno")]
        public async Task<ActionResult<List<tbMaterias>>> ObtenerMateriasSinGrupoAlumno(int alumnoId)
        {
            try
            {
                var lsMateriasAlumnoId = _context.tbAlumnosMaterias.Where(a => a.AlumnoId == alumnoId).Select(a => a.MateriaId);

                var lsMateriasSinGrupo = await _context.tbMaterias.Where(a => lsMateriasAlumnoId.Contains(a.MateriaId)).Select(a => new
                {
                    a.MateriaId,
                    a.NombreMateria,
                    a.Descripcion,
                    actividades = _context.tbActividades.Where(b=>b.MateriaId== a.MateriaId).ToList()
                }).ToListAsync();

                //foreach (var materia in lsMateriasSinGrupo)
                //{
                //    var laMaterias = lsMateriasSinGrupo.Select(a => new
                //    {
                //        a.MateriaId,
                //        a.NombreMateria,
                //        a.Descripcion,
                //        actividades = _context.tbActividades.Where(b => b.MateriaId == a.MateriaId).ToList()
                //    });

                //    lsMateriasActividades.Add(laMaterias);
                //}

                return Ok(lsMateriasSinGrupo);
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