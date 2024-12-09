using AprendeMasWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly DataContext _context;

        public AlumnosController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("AlumnoGrupoCodigo")]
        public async Task<ActionResult> RegistrarAlumnoGrupoCodigo([FromBody] AlumnoGMRegistroCodigo alumnoGrupoRegistro)
        {
            try
            {
                int alumnoId = alumnoGrupoRegistro.AlumnoId;
                string codigoAcceso = alumnoGrupoRegistro.CodigoAcceso;


                var grupoId = _context.tbGrupos.Where(a => a.CodigoAcceso == codigoAcceso).Select(a => a.GrupoId).FirstOrDefault();

                AlumnosGrupos alumnoGrupo = new()
                {
                    AlumnoId = alumnoId,
                    GrupoId = grupoId,
                };

                await _context.tbAlumnosGrupos.AddAsync(alumnoGrupo);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }


        [HttpPost("AlumnoMateriaCodigo")]
        public async Task<ActionResult> RegistrarAlumnoMateriaCodigo([FromBody] AlumnoGMRegistroCodigo alumnoMateriaRegistro)
        {
            try
            {
                int alumnoId = alumnoMateriaRegistro.AlumnoId;
                string codigoAcceso = alumnoMateriaRegistro.CodigoAcceso;


                var materiaId = _context.tbMaterias.Where(a => a.CodigoAcceso == codigoAcceso).Select(a => a.MateriaId).FirstOrDefault();

                AlumnosMaterias alumnoMateria = new()
                {
                    AlumnoId = alumnoId,
                    MateriaId = materiaId
                };

                await _context.tbAlumnosMaterias.AddAsync(alumnoMateria);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }

        [HttpPost("RegistrarAlumnoGMDocente")]
        public async Task<ActionResult> RegistrarAlumnoMateriasDocente([FromBody] AlumnoGMRegistroDocente alumnoGMRegistro)
        {
            try
            {
                List<int> lsAlumnosId = alumnoGMRegistro.AlumnosId;
                int grupoId = alumnoGMRegistro.GrupoId;
                int materiaId = alumnoGMRegistro.MateriaId;

                if (grupoId != 0)
                {
                    foreach (var id in lsAlumnosId)
                    {
                        AlumnosGrupos alumnosGrupos = new()
                        {
                            AlumnoId = id,
                            GrupoId = grupoId
                        };

                        await _context.tbAlumnosGrupos.AddAsync(alumnosGrupos);
                    }
                }
                else if (materiaId != 0)
                {
                    foreach (var id in lsAlumnosId)
                    {
                        AlumnosMaterias alumnosMaterias = new()
                        {
                            AlumnoId = id,
                            MateriaId = materiaId
                        };
                        await _context.tbAlumnosMaterias.AddAsync(alumnosMaterias);
                    }
                }
                _context.SaveChanges();

                return Ok(new {mensaje = "El alumno fue agregado correctamente"});
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }

    }
}
