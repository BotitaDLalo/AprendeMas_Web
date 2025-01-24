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


        //public async Task<ActionResult> EntregarTarea(string par)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {

        //    }
        //}


        //public async Task<ActionResult> CancelarTarea(string par)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        [HttpPost("RegistrarEnvioActividadAlumno")]
        public async Task<ActionResult> RegistrarEnvioActividadAlumno([FromBody] EntregableAlumno entregable)
        {
            try
            {
                var actividadId = entregable.ActividadId;
                var alumnoId = entregable.AlumnoId;
                var respuesta = entregable.Respuesta;
                var fechaEntrega = entregable.FechaEntrega;

                AlumnosActividades actividad = new AlumnosActividades()
                {
                    ActividadId = actividadId,
                    AlumnoId = alumnoId,
                    FechaEntrega = DateTime.Parse(fechaEntrega),
                    EstatusEntrega = true,
                    EntregablesAlumno = new EntregablesAlumno()
                    {
                        Respuesta = respuesta
                    }
                };

                _context.tbAlumnosActividades.Add(actividad);

                await _context.SaveChangesAsync();


                var datosAlumnoActividad = await _context.tbAlumnosActividades.Where(a => a.ActividadId == actividadId && a.AlumnoId == alumnoId).FirstOrDefaultAsync();


                var alumnoActividadId = datosAlumnoActividad?.AlumnoActividadId ?? 0;

                var datosEntregable = await _context.tbEntregablesAlumno.Where(a => a.AlumnoActividadId == alumnoActividadId).FirstOrDefaultAsync();

                if (datosAlumnoActividad != null && datosEntregable != null)
                {
                    return Ok(new
                    {
                        AlumnoActividadId = alumnoActividadId,
                        Respuesta = datosEntregable?.Respuesta ?? "",
                        Status = datosAlumnoActividad.EstatusEntrega
                    });
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("ObtenerEnviosActividadesAlumno")]
        public async Task<ActionResult> ObtenerEnviosActividadesAlumno(int ActividadId, int AlumnoId)
        {
            try
            {

                var datosAlumnoActividad = await _context.tbAlumnosActividades.Where(a => a.ActividadId == ActividadId && a.AlumnoId == AlumnoId).FirstOrDefaultAsync();


                var alumnoActividadId = datosAlumnoActividad?.AlumnoActividadId ?? 0;

                var datosEntregable = await _context.tbEntregablesAlumno.Where(a => a.AlumnoActividadId == alumnoActividadId).FirstOrDefaultAsync();

                if (datosAlumnoActividad != null && datosEntregable != null)
                {
                    return Ok(new
                    {
                        AlumnoActividadId = alumnoActividadId,
                        Respuesta = datosEntregable?.Respuesta ?? "",
                        Status = datosAlumnoActividad.EstatusEntrega
                    });
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("CancelarEnvioActividadAlumno")]
        public async Task<ActionResult> CancelarEnvioActividadAlumno([FromBody] CancelarEnvioActividadAlumno datosCancelacion)
        {
            try
            {
                var alumnoActividadId = datosCancelacion.AlumnoActividadId;
                var alumnoId = datosCancelacion.AlumnoId;
                var actividadId = datosCancelacion.ActividadId;

                var alumnoActividadEliminar = _context.tbAlumnosActividades.Include(a => a.EntregablesAlumno)
                    .FirstOrDefault(a => a.AlumnoActividadId == alumnoActividadId && a.AlumnoId == alumnoId);

                if (alumnoActividadEliminar != null)
                {
                    if (alumnoActividadEliminar.EntregablesAlumno != null)
                    {
                        _context.tbEntregablesAlumno.Remove(alumnoActividadEliminar.EntregablesAlumno);
                    }

                    _context.tbAlumnosActividades.Remove(alumnoActividadEliminar);
                    await _context.SaveChangesAsync();

                    var datosAlumnoActividad = await _context.tbAlumnosActividades.Where(a => a.ActividadId == actividadId && a.AlumnoId == alumnoId).FirstOrDefaultAsync();


                    //var alumnoActividadId = datosAlumnoActividad?.AlumnoActividadId ?? 0;

                    var datosEntregable = await _context.tbEntregablesAlumno.Where(a => a.AlumnoActividadId == alumnoActividadId).FirstOrDefaultAsync();

                    if (datosAlumnoActividad != null && datosEntregable != null)
                    {
                        return Ok(new
                        {
                            AlumnoActividadId = alumnoActividadId,
                            Respuesta = datosEntregable?.Respuesta ?? "",
                            Status = datosAlumnoActividad.EstatusEntrega
                        });
                    }
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
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
                        var identityId = await _userManager.GetUserIdAsync(user);

                        var alumnoExiste = _context.tbAlumnos.Any(a => a.UserId == identityId);

                        if (alumnoExiste)
                        {
                            return Ok(new { Email = email });
                        }
                        return BadRequest();
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
                        var alumnoId = await _context.tbAlumnos.Where(a => a.UserId == identityId).Select(a => a.AlumnoId).FirstOrDefaultAsync();

                        lsAlumnosId.Add(alumnoId);
                    }
                }

                int grupoId = alumnoGMRegistro.GrupoId;
                int materiaId = alumnoGMRegistro.MateriaId;



                if (grupoId != 0 && materiaId != 0)
                {
                    foreach (var aluId in lsAlumnosId)
                    {
                        bool alumnoRegistradoGrupo = _context.tbAlumnosGrupos.Any(a => a.GrupoId == grupoId && a.AlumnoId == aluId);
                        bool alumnoRegistradoMateria = _context.tbAlumnosMaterias.Any(a => a.MateriaId == materiaId && a.AlumnoId == aluId);
                        if (!alumnoRegistradoGrupo)
                        {
                            AlumnosGrupos alumnosGrupos = new()
                            {
                                AlumnoId = aluId,
                                GrupoId = grupoId
                            };
                            await _context.tbAlumnosGrupos.AddAsync(alumnosGrupos);
                        }
                        else
                        {
                            BadRequest(new { mensaje = "El alumno ya esta registrado" });
                        }

                        if (!alumnoRegistradoMateria)
                        {
                            AlumnosMaterias alumnosMaterias = new()
                            {
                                AlumnoId = aluId,
                                MateriaId = materiaId
                            };
                            await _context.tbAlumnosMaterias.AddAsync(alumnosMaterias);
                        }
                        else
                        {
                            BadRequest(new { mensaje = "El alumno ya esta registrado" });
                        }
                    }
                    _context.SaveChanges();

                    var lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);

                    return Ok(lsAlumnos);
                }
                else if (grupoId != 0)
                {
                    foreach (var aluId in lsAlumnosId)
                    {

                        bool alumnoYaRegistrado = _context.tbAlumnosGrupos.Any(a => a.GrupoId == grupoId && a.AlumnoId == aluId);
                        if (!alumnoYaRegistrado)
                        {
                            AlumnosGrupos alumnosGrupos = new()
                            {
                                AlumnoId = aluId,
                                GrupoId = grupoId
                            };

                            List<int> lsMateriasId = await _context.tbGruposMaterias.Where(a => a.GrupoId == grupoId).Select(a => a.MateriaId).ToListAsync();

                            foreach (var matId in lsMateriasId)
                            {
                                AlumnosMaterias alumnosMaterias = new()
                                {
                                    AlumnoId = aluId,
                                    MateriaId = matId
                                };

                                await _context.tbAlumnosMaterias.AddAsync(alumnosMaterias);
                            }

                            await _context.tbAlumnosGrupos.AddAsync(alumnosGrupos);
                        }
                        else
                        {
                            BadRequest(new { mensaje = "El alumno ya esta registrado" });
                        }
                    }
                    _context.SaveChanges();

                    var lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);
                    return Ok(lsAlumnos);
                }
                else if (materiaId != 0)
                {
                    foreach (var aluId in lsAlumnosId)
                    {
                        bool alumnoYaRegistrado = _context.tbAlumnosMaterias.Any(a => a.MateriaId == materiaId && a.AlumnoId == aluId);
                        if (!alumnoYaRegistrado)
                        {
                            AlumnosMaterias alumnosMaterias = new()
                            {
                                AlumnoId = aluId,
                                MateriaId = materiaId
                            };
                            await _context.tbAlumnosMaterias.AddAsync(alumnosMaterias);
                        }
                        else
                        {
                            BadRequest(new { mensaje = "El alumno ya esta registrado" });
                        }
                    }
                    _context.SaveChanges();

                    var lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);

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

                List<int> lsAlumnosId = await _context.tbAlumnosGrupos.Where(a => a.GrupoId == grupoId).Select(a => a.AlumnoId).ToListAsync();

                List<EmailVerificadoAlumno> lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);

                return Ok(lsAlumnos);
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }

        [HttpPost("ObtenerListaAlumnosMateria")]
        public async Task<ActionResult> ObtenerListaAlumnosMateria([FromBody] Indices indice)
        {
            try
            {
                int materiaId = indice.MateriaId;

                List<int> lsAlumnosId = await _context.tbAlumnosMaterias.Where(a => a.MateriaId == materiaId).Select(a => a.AlumnoId).ToListAsync();

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
