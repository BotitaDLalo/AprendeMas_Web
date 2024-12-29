using AprendeMasWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController(UserManager<IdentityUser> userManager, DataContext context) : ControllerBase
    {
        private readonly DataContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;


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


        [HttpPost("VerificarAlumnoEmail")]
        public async Task<ActionResult> VerificarAlumnoEmail([FromBody] EmailVerificadoAlumno verifyEmail)
        {
            try
            {
                var email = verifyEmail.Email;
                if (!email.IsNullOrEmpty())
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        return Ok(new { Email = email });
                    }
                }
                return BadRequest(new { Email = email });

            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "Correo no valido" });
            }
        }

        [HttpPost("RegistrarAlumnoGMDocente")]
        public async Task<ActionResult> RegistrarAlumnoMateriasDocente([FromBody] AlumnoGMRegistroDocente alumnoGMRegistro)
        {
            try
            {
                List<string> lsEmails = alumnoGMRegistro.Emails;
                List<int> lsAlumnosId = [];

                foreach (var email in lsEmails)
                {
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user != null)
                    {
                        var identityId = await _userManager.GetUserIdAsync(user);
                        var alumnoId = await _context.tbAlumnos.Where(a=>a.UserId == identityId).Select(a=>a.AlumnoId).FirstOrDefaultAsync();

                        lsAlumnosId.Add(alumnoId);
                    }
                }

                int grupoId = alumnoGMRegistro.GrupoId;
                int materiaId = alumnoGMRegistro.MateriaId;

                if (grupoId != 0)
                {
                    foreach (var id in lsAlumnosId)
                    {
                        if (id != 0)
                        {
                            AlumnosGrupos alumnosGrupos = new()
                            {
                                AlumnoId = id,
                                GrupoId = grupoId
                            };

                            await _context.tbAlumnosGrupos.AddAsync(alumnosGrupos);
                        }
                    }
                    _context.SaveChanges();

                    //List<EmailVerificadoAlumno> lsAlumnos = [];
                    //foreach (var id in lsAlumnosId)
                    //{
                    //    var alumnoDatos = _context.tbAlumnos.Where(a => a.AlumnoId == id).FirstOrDefault();
                    //    if (alumnoDatos != null)
                    //    {
                    //        var userName = await _userManager.FindByIdAsync(alumnoDatos.UserId);

                    //        EmailVerificadoAlumno alumno = new()
                    //        {
                    //            Email = userName?.Email ?? "",
                    //            UserName = userName?.UserName ?? "",
                    //            Nombre = alumnoDatos.Nombre,
                    //            ApellidoPaterno = alumnoDatos.ApellidoPaterno,
                    //            ApellidoMaterno = alumnoDatos.ApellidoMaterno,
                    //        };

                    //        lsAlumnos.Add(alumno);
                    //    }

                    //}

                    var lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);
                    return Ok(lsAlumnos);
                }
                else if (materiaId != 0)
                {
                    foreach (var id in lsAlumnosId)
                    {
                        if (id != 0)
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

                    var lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);
                    //List<EmailVerificadoAlumno> lsAlumnos = [];
                    //foreach (var id in lsAlumnosId)
                    //{
                    //    var alumnoDatos = _context.tbAlumnos.Where(a => a.AlumnoId == id).FirstOrDefault();
                    //    if (alumnoDatos != null)
                    //    {
                    //        var userName = await _userManager.FindByIdAsync(alumnoDatos.UserId);

                    //        EmailVerificadoAlumno alumno = new()
                    //        {
                    //            Email = userName?.Email ?? "",
                    //            UserName = userName?.UserName ?? "",
                    //            Nombre = alumnoDatos.Nombre,
                    //            ApellidoPaterno = alumnoDatos.ApellidoPaterno,
                    //            ApellidoMaterno = alumnoDatos.ApellidoMaterno,
                    //        };

                    //        lsAlumnos.Add(alumno);
                    //    }

                    //}

                    return Ok(lsAlumnos);
                }


                //TODO: Retornar UserName, Nombre, Apellido Paterno, ApellidoMaterno
                //return Ok(new { mensaje = "El alumno fue agregado correctamente" });
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }


        [HttpPost("ObtenerListaAlumnosGrupo")]
        public async Task<ActionResult> ObtenerListaAlumnosGrupo([FromBody] Indices indice)
        {
            try
            {
                int grupoId = indice.GrupoId;

                List<int> lsAlumnosId = await _context.tbAlumnosGrupos.Where(a=>a.GrupoId == grupoId).Select(a=>a.AlumnoId).ToListAsync();

                List<EmailVerificadoAlumno> lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);

                return Ok(lsAlumnos);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }


        private async Task<List<EmailVerificadoAlumno>> ObtenerListaAlumnos(List<int> lsAlumnosId)
        {
            try
            {
                List<EmailVerificadoAlumno> lsAlumnos = [];
                foreach (var id in lsAlumnosId)
                {
                    var alumnoDatos = _context.tbAlumnos.Where(a => a.AlumnoId == id).FirstOrDefault();
                    if (alumnoDatos != null)
                    {
                        var userName = await _userManager.FindByIdAsync(alumnoDatos.UserId);

                        EmailVerificadoAlumno alumno = new()
                        {
                            Email = userName?.Email ?? "",
                            UserName = userName?.UserName ?? "",
                            Nombre = alumnoDatos.Nombre,
                            ApellidoPaterno = alumnoDatos.ApellidoPaterno,
                            ApellidoMaterno = alumnoDatos.ApellidoMaterno,
                        };

                        lsAlumnos.Add(alumno);
                    }

                }

                return lsAlumnos;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
