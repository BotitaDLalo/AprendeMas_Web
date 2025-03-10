using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit.Tnef;
using AprendeMasWeb.Recursos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Configuration;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.Autenticacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSigninController(FuncionesGenerales fg,IConfiguration configuration, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DataContext context) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly DataContext _context = context;
        private readonly FuncionesGenerales _fg = fg;


        [HttpPost("IniciarSesionGoogle")]
        public async Task<IActionResult> IniciarSesionGoogle([FromBody] RegistrarUsuarioGoogle usuario)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(usuario.IdToken);

                if (payload == null)
                {
                    return BadRequest("idToken invalido");
                }

                var userName = payload.FamilyName + payload.GivenName;
                var Email = payload.Email;
                var Token = usuario.IdToken;

                var user = await _userManager.FindByEmailAsync(Email);

                if (user != null)
                {
                    var rol = await _userManager.GetRolesAsync(user);
                    var identityUserId = await _userManager.GetUserIdAsync(user);
                    var rolUsuario = rol.FirstOrDefault();

                    if (rolUsuario == null) return Ok(new AutenticacionRespuesta
                    {
                        Correo = Email,
                        Token = Token,
                        EstaAutorizado = EstatusAutorizacion.PENDIENTE,
                        RequiereDatosAdicionales = RequiereDatosAdicionales.REQUERIDO
                    });

                    if (rolUsuario == "Docente")
                    {
                        var docente = _context.tbDocentes.Where(a => a.UserId == identityUserId).FirstOrDefault();
                        if (docente == null) return BadRequest();

                        var respuesta = docente.estaAutorizado switch
                        {
                            null => new AutenticacionRespuesta
                            {
                                Id = docente.DocenteId,
                                Correo = Email,
                                Token = Token,
                                EstaAutorizado = EstatusAutorizacion.PENDIENTE,
                                RequiereDatosAdicionales = RequiereDatosAdicionales.NO_REQUERIDO
                            },
                            false => new AutenticacionRespuesta
                            {
                                EstaAutorizado = EstatusAutorizacion.DENEGADO
                            },
                            true => new AutenticacionRespuesta
                            {
                                Id = docente.DocenteId,
                                UserName = userName,
                                Correo = Email,
                                Rol = rolUsuario,
                                Token = Token,
                                EstaAutorizado = EstatusAutorizacion.AUTORIZADO,
                                RequiereDatosAdicionales = RequiereDatosAdicionales.NO_REQUERIDO
                            }
                        };

                        return Ok(respuesta);

                    }
                    else if (rolUsuario == "Alumno")
                    {
                        var alumno = await _context.tbAlumnos.Where(a => a.UserId == identityUserId).FirstOrDefaultAsync();
                        if (alumno == null) return BadRequest();

                        return Ok(new AutenticacionRespuesta
                        {
                            Id = alumno.AlumnoId,
                            UserName = userName,
                            Correo = Email,
                            Rol = rolUsuario,
                            Token = Token,
                            EstaAutorizado = EstatusAutorizacion.AUTORIZADO
                        });
                    }





                }


                //CREAR USUARIO
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = Email
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return Ok(new AutenticacionRespuesta
                {
                    Correo = Email,
                    Token = Token,
                    EstaAutorizado = EstatusAutorizacion.PENDIENTE,
                    RequiereDatosAdicionales = RequiereDatosAdicionales.REQUERIDO
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpPost("RegistrarDatosFaltantesGoogle")]
        public async Task<IActionResult> RegistrarDatosFaltantesGoogle([FromBody] DatosFaltantesGoogle datos)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                int idUsuario = 0;
                var nombres = datos.Nombres;
                var apellidoPaterno = datos.ApellidoPaterno;
                var apellidoMaterno = datos.ApellidoMaterno;
                var role = datos.Role;
                var token = datos.IdToken;


                var payload = await GoogleJsonWebSignature.ValidateAsync(datos.IdToken);
                if (payload == null) return BadRequest("idToken invalido");



                var email = payload.Email;
                var userName = payload.GivenName;

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null) return BadRequest();


                //CREAR ROL
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }


                //ASIGNAR ROL
                var tieneRol = await _userManager.IsInRoleAsync(user, role);
                if (!tieneRol)
                {
                    var asignarRol = await _userManager.AddToRoleAsync(user, role);
                    if (!asignarRol.Succeeded) return BadRequest(asignarRol.Errors);
                }


                if (role == "Docente")
                {
                    var fcmToken = datos.FcmToken;
                    if (fcmToken == null) return BadRequest();

                    var identityUserId = await _userManager.GetUserIdAsync(user);
                    DateTime fechaExpiracionCodigo = DateTime.UtcNow.AddMinutes(59);
                    string codigo = RecursosGenerales.GenerarCodigoAleatorio();
                    Docentes docente = new()
                    {
                        Nombre = nombres,
                        ApellidoPaterno = apellidoPaterno,
                        ApellidoMaterno = apellidoMaterno,
                        UserId = identityUserId,
                        CodigoAutorizacion = codigo,
                        FechaExpiracionCodigo = fechaExpiracionCodigo,
                    };

                    _context.tbDocentes.Add(docente);
                    await _context.SaveChangesAsync();


                    await _fg.RegistrarFcmTokenUsuario(identityUserId, fcmToken);



                    return Ok(new
                    {
                        estaAutorizado = EstatusAutorizacion.PENDIENTE,
                        requiereDatosAdicionales = RequiereDatosAdicionales.NO_REQUERIDO
                    });
                }
                else if (role == "Alumno")
                {
                    var fcmToken = datos.FcmToken;
                    if (fcmToken == null) return BadRequest();

                    var identityUserId = await _userManager.GetUserIdAsync(user);

                    Alumnos alumno = new()
                    {
                        Nombre = nombres,
                        ApellidoPaterno = apellidoPaterno,
                        ApellidoMaterno = apellidoMaterno,
                        UserId = identityUserId
                    };
                    _context.tbAlumnos.Add(alumno);
                    await _context.SaveChangesAsync();
                    idUsuario = _context.tbAlumnos.Where(a => a.UserId == identityUserId).Select(a => a.AlumnoId).FirstOrDefault();
                    int alumnoId = alumno.AlumnoId;

                    await _fg.RegistrarFcmTokenUsuario(identityUserId, token);

                    var emailEncontrado = await _userManager.FindByIdAsync(identityUserId);
                    if (emailEncontrado == null) return BadRequest();

                    return Ok(new AutenticacionRespuesta
                    {
                        Id = alumnoId,
                        UserName = userName,
                        Correo = email,
                        Rol = role,
                        Token = token,
                        EstaAutorizado = EstatusAutorizacion.AUTORIZADO
                    });




                }
                return BadRequest();

            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }


        [HttpPost("ValidarCodigoDocenteGoogle")]
        public async Task<IActionResult> ValidarCodigoAutorizacionDocente([FromBody] ValidarCodigoDocente datos)
        {
            try
            {
                var email = datos.Email;
                var codigoValidar = datos.CodigoValidar;
                var token = datos.IdToken ?? "";

                var emailEncontrado = await _userManager.FindByEmailAsync(email);
                if (emailEncontrado == null)
                {
                    return BadRequest(new
                    {
                        ErrorCode = ErrorCatalogo.ErrorCodigos.CredencialesInvalidas,
                        ErrorMessage = ErrorCatalogo.GetMensajeError(ErrorCatalogo.ErrorCodigos.CredencialesInvalidas)
                    });
                }

                //Obteniendo rol del usuario
                var rol = await _userManager.GetRolesAsync(emailEncontrado);
                var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");

                var identityUserId = emailEncontrado.Id;

                var docente = _context.tbDocentes.Where(a => a.UserId == identityUserId).FirstOrDefault();
                if (docente != null)
                {
                    int idUsuario = docente.DocenteId;

                    if (codigoValidar != docente.CodigoAutorizacion)
                    {
                        return BadRequest(new { ErrorCode = ErrorCatalogo.ErrorCodigos.codigoAutorizacionInvalido });
                    }
                    //5:10                              5:15
                    if (docente.FechaExpiracionCodigo < DateTime.Now)
                    {
                        return BadRequest(new { ErrorCode = ErrorCatalogo.ErrorCodigos.codigoAutorizacionExpirado });
                    }

                    docente.FechaExpiracionCodigo = null;
                    docente.CodigoAutorizacion = null;
                    docente.estaAutorizado = true;
                    _context.SaveChanges();

                    return Ok(new AutenticacionRespuesta
                    {
                        Id = idUsuario,
                        UserName = emailEncontrado.UserName,
                        Correo = emailEncontrado.Email,
                        Rol = rolUsuario,
                        Token = token,
                        EstaAutorizado = EstatusAutorizacion.AUTORIZADO,
                    });
                }

                return BadRequest(new { mensaje = "El docente no existe." });
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }


        [HttpPost("VerificarIdToken")]
        public async Task<IActionResult> VerificarGoogleIdToken([FromBody] VerificarGoogleIdToken token)
        {
            try
            {
                var Token = token.IdToken;

                var payload = await GoogleJsonWebSignature.ValidateAsync(Token);
                if (payload != null && payload.ExpirationTimeSeconds > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    var Name = payload.Name;
                    var Email = payload.Email;

                    var user = await _userManager.FindByEmailAsync(Email);
                    if (user == null)
                    {
                        return BadRequest();
                    }

                    int idUsuario = 0;
                    var identityUserId = await _userManager.GetUserIdAsync(user);
                    var rol = await _userManager.GetRolesAsync(user);
                    var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");


                    if (rolUsuario == "Docente")
                    {
                        idUsuario = _context.tbDocentes.Where(a => a.UserId == identityUserId).Select(a => a.DocenteId).FirstOrDefault();
                    }
                    else if (rolUsuario == "Alumno")
                    {
                        idUsuario = _context.tbAlumnos.Where(a => a.UserId == identityUserId).Select(a => a.AlumnoId).FirstOrDefault();
                    }

                    return Ok(new AutenticacionRespuesta
                    {
                        Id = idUsuario,
                        UserName = Name,
                        Correo = Email,
                        Rol = rolUsuario,
                        Token = Token
                    });
                }
                return BadRequest(new { mensaje = "El token no es valido" });

            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "El token no es valido" });

            }



        }
    }
}
