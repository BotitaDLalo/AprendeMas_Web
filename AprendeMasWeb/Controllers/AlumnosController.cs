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


        /////////////WEB////////////

        // Endpoint para unirse a una clase con código de acceso
        [HttpPost("UnirseAClase")]
        public async Task<IActionResult> UnirseAClase([FromBody] UnirseAClaseRequest request)
        {
            if (string.IsNullOrEmpty(request.CodigoAcceso))
            {
                return BadRequest(new { mensaje = "El código de acceso es obligatorio" });
            }

            // Buscar si el código pertenece a un Grupo
            var grupo = await _context.tbGrupos.FirstOrDefaultAsync(g => g.CodigoAcceso == request.CodigoAcceso);

            if (grupo != null)
            {
                // Verificar si el alumno ya está inscrito en el grupo
                var existeRelacion = await _context.tbAlumnosGrupos
                    .AnyAsync(ag => ag.AlumnoId == request.AlumnoId && ag.GrupoId == grupo.GrupoId);

                if (!existeRelacion)
                {
                    // Agregar el alumno al grupo
                    var nuevaRelacion = new tbAlumnosGrupos
                    {
                        AlumnoId = request.AlumnoId,
                        GrupoId = grupo.GrupoId
                    };
                    _context.tbAlumnosGrupos.Add(nuevaRelacion);
                    await _context.SaveChangesAsync();
                    return Ok(new { mensaje = "Te has unido al grupo", nombre = grupo.NombreGrupo, esGrupo = true });
                }

            }

            // Buscar si el código pertenece a una Materia
            var materia = await _context.tbMaterias.FirstOrDefaultAsync(m => m.CodigoAcceso == request.CodigoAcceso);

            if (materia != null)
            {
                // Verificar si el alumno ya está inscrito en la materia
                var existeRelacion = await _context.tbAlumnosMaterias
                    .AnyAsync(am => am.AlumnoId == request.AlumnoId && am.MateriaId == materia.MateriaId);

                if (!existeRelacion)
                {
                    // Agregar el alumno a la materia
                    var nuevaRelacion = new tbAlumnosMaterias
                    {
                        AlumnoId = request.AlumnoId,
                        MateriaId = materia.MateriaId
                    };
                    _context.tbAlumnosMaterias.Add(nuevaRelacion);
                    await _context.SaveChangesAsync();
                    return Ok(new { mensaje = "Te has unido a la materia", nombre = materia.NombreMateria, esGrupo = false });
                }

            }

            return NotFound(new { mensaje = "Código de acceso no válido" });
        }

        /////////////Termina WEB////////////


        [HttpPost("UnirseAClaseM")]
        public async Task<IActionResult> UnirseAClaseM([FromBody] UnirseAClaseRequest request)
        {
            try
            {
                var codigo = request.CodigoAcceso;

                var grupo = await _context.tbGrupos.FirstOrDefaultAsync(g => g.CodigoAcceso == request.CodigoAcceso);

                if (grupo != null)
                {
                    int docenteId = grupo.DocenteId;
                    var docente = await _context.tbDocentes.Where(a => a.DocenteId == docenteId).FirstOrDefaultAsync();

                    if (docente == null) return BadRequest();

                    var existeRelacion = await _context.tbAlumnosGrupos
                        .AnyAsync(ag => ag.AlumnoId == request.AlumnoId && ag.GrupoId == grupo.GrupoId);

                    if (!existeRelacion)
                    {
                        var lsMateriasId = await _context.tbGruposMaterias.Where(a => a.GrupoId == grupo.GrupoId).Select(a => a.MateriaId).ToListAsync();

                        var lsMaterias = await _context.tbMaterias.Where(a => lsMateriasId.Contains(a.MateriaId)).Select(m => new MateriaRes
                        {
                            MateriaId = m.MateriaId,
                            NombreMateria = m.NombreMateria,
                            Descripcion = m.Descripcion,
                            //m.CodigoColor,
                            Actividades = _context.tbActividades.Where(a => a.MateriaId == m.MateriaId).ToList()
                        }).ToListAsync();


                        GrupoRes grupoRes = new()
                        {
                            GrupoId = grupo.GrupoId,
                            NombreGrupo = grupo.NombreGrupo,
                            Descripcion = grupo.Descripcion,
                            CodigoAcceso = grupo.CodigoAcceso,
                            CodigoColor = grupo.CodigoColor,
                            Materias = lsMaterias
                        };

                        var nuevaRelacion = new tbAlumnosGrupos
                        {
                            AlumnoId = request.AlumnoId,
                            GrupoId = grupo.GrupoId
                        };
                        _context.tbAlumnosGrupos.Add(nuevaRelacion);
                        await _context.SaveChangesAsync();


                        UnirseAClaseMRespuesta respuesta = new()
                        {
                            Grupo = grupoRes,
                            EsGrupo = true
                        };


                        return Ok(respuesta);
                    }
                    return BadRequest();

                }

                var materia = await _context.tbMaterias.FirstOrDefaultAsync(m => m.CodigoAcceso == request.CodigoAcceso);

                if (materia != null)
                {
                    int docenteId = materia.DocenteId;
                    var docente = await _context.tbDocentes.Where(a => a.DocenteId == docenteId).FirstOrDefaultAsync();

                    if (docente == null) return BadRequest();
                    var existeRelacion = await _context.tbAlumnosMaterias
                         .AnyAsync(am => am.AlumnoId == request.AlumnoId && am.MateriaId == materia.MateriaId);

                    if (!existeRelacion)
                    {
                        MateriaRes materiaRes = new()
                        {
                            MateriaId = materia.MateriaId,
                            NombreMateria = materia.NombreMateria,
                            Descripcion = materia.Descripcion,
                            Actividades = await _context.tbActividades.Where(a => a.MateriaId == materia.MateriaId).ToListAsync()
                        };

                        var nuevaRelacion = new tbAlumnosMaterias
                        {
                            AlumnoId = request.AlumnoId,
                            MateriaId = materia.MateriaId
                        };
                        _context.tbAlumnosMaterias.Add(nuevaRelacion);
                        await _context.SaveChangesAsync();

                        UnirseAClaseMRespuesta respuesta = new()
                        {
                            Materia = materiaRes,
                            EsGrupo = false
                        };

                        return Ok(respuesta);
                    }
                    return BadRequest();
                }

                return NotFound(new { mensaje = "No existe la clase." });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("RegistrarEnvioActividadAlumno")]
        public async Task<ActionResult> RegistrarEnvioActividadAlumno([FromBody] EntregableAlumno entregable)
        {
            try
            {
                var actividadId = entregable.ActividadId;
                var alumnoId = entregable.AlumnoId;
                var respuesta = entregable.Respuesta;
                var fechaEntrega = entregable.FechaEntrega;

                tbAlumnosActividades actividad = new tbAlumnosActividades()
                {
                    ActividadId = actividadId,
                    AlumnoId = alumnoId,
                    FechaEntrega = DateTime.Parse(fechaEntrega),
                    EstatusEntrega = true,
                    EntregablesAlumno = new tbEntregablesAlumno()
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
                        EntregaId = datosEntregable.EntregaId,
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

                var datosAlumnoActividad = await _context.tbAlumnosActividades.Where(a => a.ActividadId == ActividadId && a.AlumnoId == AlumnoId).Select(a => new { a.AlumnoActividadId, a.FechaEntrega, a.EstatusEntrega }).FirstOrDefaultAsync();


                var alumnoActividadId = datosAlumnoActividad?.AlumnoActividadId ?? 0;

                var fechaEntrega = datosAlumnoActividad?.FechaEntrega;

                var datosEntregable = await _context.tbEntregablesAlumno.Where(a => a.AlumnoActividadId == alumnoActividadId).FirstOrDefaultAsync();

                if (datosAlumnoActividad != null && datosEntregable != null)
                {
                    return Ok(new
                    {
                        EntregaId = datosEntregable.EntregaId,
                        AlumnoActividadId = alumnoActividadId,
                        Respuesta = datosEntregable?.Respuesta ?? "",
                        Status = datosAlumnoActividad.EstatusEntrega,
                        FechaEntrega = fechaEntrega
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

                tbAlumnosGrupos alumnoGrupo = new()
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

                tbAlumnosMaterias alumnoMateria = new()
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


                //TODO: EN CASO DE REGISTRAR UN ALUMNO A UNA MATERIA CON UN GRUPO
                //if (grupoId != 0 && materiaId != 0)
                //{
                //    foreach (var aluId in lsAlumnosId)
                //    {
                //        bool alumnoRegistradoGrupo = _context.tbAlumnosGrupos.Any(a => a.GrupoId == grupoId && a.AlumnoId == aluId);
                //        bool alumnoRegistradoMateria = _context.tbAlumnosMaterias.Any(a => a.MateriaId == materiaId && a.AlumnoId == aluId);
                //        if (!alumnoRegistradoGrupo)
                //        {
                //            AlumnosGrupos alumnosGrupos = new()
                //            {
                //                AlumnoId = aluId,
                //                GrupoId = grupoId
                //            };
                //            await _context.tbAlumnosGrupos.AddAsync(alumnosGrupos);
                //        }
                //        else
                //        {
                //            BadRequest(new { mensaje = "El alumno ya esta registrado" });
                //        }

                //        if (!alumnoRegistradoMateria)
                //        {
                //            AlumnosMaterias alumnosMaterias = new()
                //            {
                //                AlumnoId = aluId,
                //                MateriaId = materiaId
                //            };
                //            await _context.tbAlumnosMaterias.AddAsync(alumnosMaterias);
                //        }
                //        else
                //        {
                //            BadRequest(new { mensaje = "El alumno ya esta registrado" });
                //        }
                //    }
                //    _context.SaveChanges();

                //    var lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);

                //    return Ok(lsAlumnos);
                //}
                //else 
                if (grupoId != 0)
                {
                    foreach (var aluId in lsAlumnosId)
                    {

                        bool alumnoYaRegistrado = _context.tbAlumnosGrupos.Any(a => a.GrupoId == grupoId && a.AlumnoId == aluId);
                        if (!alumnoYaRegistrado)
                        {
                            tbAlumnosGrupos alumnosGrupos = new()
                            {
                                AlumnoId = aluId,
                                GrupoId = grupoId
                            };

                            //List<int> lsMateriasId = await _context.tbGruposMaterias.Where(a => a.GrupoId == grupoId).Select(a => a.MateriaId).ToListAsync();

                            //foreach (var matId in lsMateriasId)
                            //{
                            //    AlumnosMaterias alumnosMaterias = new()
                            //    {
                            //        AlumnoId = aluId,
                            //        MateriaId = matId
                            //    };

                            //    await _context.tbAlumnosMaterias.AddAsync(alumnosMaterias);
                            //}

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
                            tbAlumnosMaterias alumnosMaterias = new()
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
                int grupoId = indice.GrupoId;
                int materiaId = indice.MateriaId;

                if (grupoId > 0 && materiaId > 0)
                {
                    List<int> lsAlumnosGruposId = await _context.tbAlumnosGrupos.Where(a => a.GrupoId == grupoId).Select(a => a.AlumnoId).ToListAsync();
                    List<EmailVerificadoAlumno> lsAlumnos = await ObtenerListaAlumnos(lsAlumnosGruposId);
                    return Ok(lsAlumnos);
                }
                else
                {
                    List<int> lsAlumnosId = await _context.tbAlumnosMaterias.Where(a => a.MateriaId == materiaId).Select(a => a.AlumnoId).ToListAsync();

                    List<EmailVerificadoAlumno> lsAlumnos = await ObtenerListaAlumnos(lsAlumnosId);

                    return Ok(lsAlumnos);
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }

        //public async Task<ActionResult> EliminarAlumnoMateria([FromBody])
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

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
