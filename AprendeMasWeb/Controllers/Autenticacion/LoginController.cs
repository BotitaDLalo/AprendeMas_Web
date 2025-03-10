using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using AprendeMasWeb.Models;
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using AprendeMasWeb.Recursos;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AprendeMasWeb.Controllers.Autenticacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(FuncionesGenerales fg, IEmailSender emailSender, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, DataContext context) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly DataContext _context = context;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly FuncionesGenerales _fg = fg;


        [HttpPost("VerificarTokenFcm")]
        public async Task<ActionResult> VerificarTokenFcm(int id, string fcmToken, string role)
        {
            try
            {
                //bool existeToken = await _context.tbAlumnosTokens.AnyAsync(a => a.Token == fcmToken);

                bool existeToken = await _context.tbUsuariosFcmTokens.AnyAsync(a => a.Token == fcmToken);
                if (existeToken)
                {
                    return Ok(new { Mensaje = $"El token del alumno con Id ${id} existe" });
                }
                else
                {
                    var identityUserId = "";

                    if (role == Recursos.Roles.DOCENTE)
                    {
                        identityUserId = _context.tbDocentes.Where(a => a.DocenteId == id).Select(a => a.UserId).FirstOrDefault();
                    }
                    else if (role == Recursos.Roles.ALUMNO)
                    {
                        identityUserId = _context.tbAlumnos.Where(a => a.AlumnoId == id).Select(a => a.UserId).FirstOrDefault();
                    }

                    if (identityUserId == null) return BadRequest();

                    await _fg.RegistrarFcmTokenUsuario(identityUserId, fcmToken);

                    return Ok(new { Mensaje = $"El token del usuario con Id ${id} existe" });
                }

            }
            catch (Exception)
            {
                return BadRequest(new { Mensaje = "No se pudo verificar el token." });
            }
        }

        [HttpPost("RegistroUsuario")]
        public async Task<IActionResult> RegistroUsuario([FromBody] UsuarioRegistro modelo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var nombreUsuario = modelo.NombreUsuario;
                    var nombreUsuarioEncontrado = await _userManager.FindByNameAsync(nombreUsuario!);

                    if (nombreUsuarioEncontrado != null)
                    {
                        return BadRequest(new
                        {
                            ErrorCode = ErrorCatalogo.ErrorCodigos.nombreUsuarioUsado,
                            ErrorMessage = ErrorCatalogo.GetMensajeError(ErrorCatalogo.ErrorCodigos.nombreUsuarioUsado)
                        });
                    }


                    var userName = modelo.NombreUsuario;
                    var email = modelo.Correo;
                    var rol = modelo.TipoUsuario;

                    var usuario = new IdentityUser()
                    {
                        UserName = userName,
                        Email = email
                    };

                    var usuarioRegistro = await _userManager.CreateAsync(usuario, modelo.Clave);
                    if (!usuarioRegistro.Succeeded)
                    {
                        return BadRequest(usuarioRegistro.Errors);
                    }

                    if (!await _roleManager.RoleExistsAsync(modelo.TipoUsuario))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(modelo.TipoUsuario));
                    }

                    var asignarRol = await _userManager.AddToRoleAsync(usuario, rol);
                    if (!asignarRol.Succeeded)
                    {
                        return BadRequest(asignarRol.Errors);
                    }

                    if (modelo.TipoUsuario == Recursos.Roles.DOCENTE)
                    {
                        var fcmToken = modelo.FcmToken;

                        if (fcmToken == null) return BadRequest();

                        var identityUserId = await _userManager.GetUserIdAsync(usuario);
                        DateTime fechaExpiracionCodigo = DateTime.UtcNow.AddMinutes(59);
                        string codigo = RecursosGenerales.GenerarCodigoAleatorio();
                        Docentes docentes = new()
                        {
                            ApellidoPaterno = modelo.ApellidoPaterno,
                            ApellidoMaterno = modelo.ApellidoMaterno,
                            Nombre = modelo.Nombre,
                            UserId = identityUserId.ToString(),
                            CodigoAutorizacion = codigo,
                            FechaExpiracionCodigo = fechaExpiracionCodigo,
                        };
                        _context.tbDocentes.Add(docentes);
                        _context.SaveChanges();

                        await _fg.RegistrarFcmTokenUsuario(identityUserId, fcmToken);

                        return Ok(new AutenticacionRespuesta
                        {
                            EstaAutorizado = EstatusAutorizacion.PENDIENTE
                        });

                    }
                    else if (modelo.TipoUsuario == Recursos.Roles.ALUMNO)
                    {
                        var fcmToken = modelo.FcmToken;

                        if (fcmToken == null) return BadRequest();

                        var identityUserId = await _userManager.GetUserIdAsync(usuario);

                        Alumnos alumnos = new()
                        {
                            ApellidoPaterno = modelo.ApellidoPaterno,
                            ApellidoMaterno = modelo.ApellidoMaterno,
                            Nombre = modelo.Nombre,
                            UserId = identityUserId,
                        };
                        await _context.tbAlumnos.AddAsync(alumnos);


                        await _context.SaveChangesAsync();

                        await _fg.RegistrarFcmTokenUsuario(identityUserId, fcmToken);

                        var emailEncontrado = await _userManager.FindByIdAsync(identityUserId);

                        if (emailEncontrado == null) return BadRequest();

                        var tokenString = _fg.GenerarJwt(alumnos.AlumnoId, emailEncontrado, rol);

                        return Ok(new AutenticacionRespuesta
                        {
                            Id = alumnos.AlumnoId,
                            UserName = userName,
                            Correo = email,
                            Rol = rol,
                            Token = tokenString,
                            EstaAutorizado = EstatusAutorizacion.AUTORIZADO
                        });


                    }
                    else if (modelo.TipoUsuario == Recursos.Roles.ADMINISTRADOR)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(new { Mensaje = "Hubo un error en el registro" });

                    }
                    //return Ok(new { nombre = userName, correo = email, rol });
                }
                return BadRequest(new { Mensaje = "Hubo un error en el registro" });

            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });

            }

        }




        [HttpPost("InicioSesionUsuario")]
        public async Task<IActionResult> InicioSesionUsuario([FromBody] UsuarioInicioSesion model)
        {
            try
            {
                //Verificar si existe el usuario
                var emailEncontrado = await _userManager.FindByEmailAsync(model.Correo);
                if (emailEncontrado == null)
                {
                    return BadRequest(new
                    {
                        ErrorCode = ErrorCatalogo.ErrorCodigos.CredencialesInvalidas,
                        ErrorMessage = ErrorCatalogo.GetMensajeError(ErrorCatalogo.ErrorCodigos.CredencialesInvalidas)
                    });
                }

                //Verificar password
                var user = await _signInManager.CheckPasswordSignInAsync(emailEncontrado, model.Clave, lockoutOnFailure: false);
                if (!user.Succeeded)
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



                int idUsuario = 0;
                var identityUserId = await _userManager.GetUserIdAsync(emailEncontrado);

                if (rolUsuario == "Docente")
                {

                    //idUsuario = _context.tbDocentes.Where(a => a.UserId == identityUserId).Select(a => a.DocenteId).FirstOrDefault();
                    var docente = _context.tbDocentes.Where(a => a.UserId == identityUserId).FirstOrDefault();
                    if (docente != null)
                    {
                        if (docente.estaAutorizado == null)
                        {
                            return Ok(new AutenticacionRespuesta
                            {
                                EstaAutorizado = EstatusAutorizacion.PENDIENTE
                            });
                        }
                        else
                        {
                            if (!docente.estaAutorizado.Value)
                            {
                                return Ok(new AutenticacionRespuesta
                                {
                                    EstaAutorizado = EstatusAutorizacion.DENEGADO
                                });
                            }
                            else
                            {
                                idUsuario = docente.DocenteId;
                            }
                        }
                    }
                }
                else if (rolUsuario == "Alumno")
                {
                    idUsuario = _context.tbAlumnos.Where(a => a.UserId == identityUserId).Select(a => a.AlumnoId).FirstOrDefault();
                }

                var tokenString = _fg.GenerarJwt(idUsuario, emailEncontrado, rolUsuario);


                return Ok(new AutenticacionRespuesta
                {
                    Id = idUsuario,
                    UserName = emailEncontrado.UserName,
                    Correo = emailEncontrado.Email,
                    Rol = rolUsuario,
                    Token = tokenString,
                    EstaAutorizado = EstatusAutorizacion.AUTORIZADO
                });
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }

        }


        [HttpPost("ValidarCodigoDocente")]
        public async Task<IActionResult> ValidarCodigoAutorizacionDocente([FromBody] ValidarCodigoDocente datos)
        {
            try
            {
                string email = datos.Email;
                string codigoValidar = datos.CodigoValidar;

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

                    var tokenString = _fg.GenerarJwt(idUsuario, emailEncontrado, rolUsuario);
                    return Ok(new
                    {
                        Id = idUsuario,
                        userName = emailEncontrado.UserName,
                        correo = emailEncontrado.Email,
                        rol = rolUsuario,
                        token = tokenString,
                        estaAutorizado = EstatusAutorizacion.AUTORIZADO,
                    });
                }

                return BadRequest(new { mensaje = "El docente no existe." });
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpGet("VerificarToken")]
        public IActionResult VerificarJWT(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { mensaje = "El token es requerido" });
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();

                var confSecretKey = _configuration["jwt:SecretKey"];
                var jwt = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(confSecretKey ?? throw new ArgumentNullException(confSecretKey, "Token no configurado")));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwt,
                    ValidateLifetime = true,
                    ValidIssuer = "Aprende_Mas",
                    ValidAudience = "Aprende_Mas",
                    ClockSkew = TimeSpan.Zero
                };

                var claimsPrincipal = handler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                var idUsuario = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
                var userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? "No existe nombre";
                var correo = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? "No existe correo";
                var rol = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value ?? "No existe rol";

                return Ok(new AutenticacionRespuesta
                {
                    Id = int.Parse(idUsuario),
                    UserName = userName,
                    Correo = correo,
                    Rol = rol,
                    Token = token
                });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { mensaje = "El token ha expirado" });
            }
            catch (Exception)
            {
                return Unauthorized(new { mensaje = "El token es inválido" });
            }

        }

        [HttpPost("VerificarEmailUsuario")]
        public async Task<IActionResult> VerificarEmailUsuario([FromBody]string email)
        {
            try
            {
                var emailEsValido = await _userManager.FindByEmailAsync(email);

                if (emailEsValido == null)
                {
                    HttpContext.Session.SetString(Recursos.SessionKeys.Email, email);
                    return Ok();
                }
                var codigoError = ErrorCatalogo.ErrorCodigos.CorreoUsuarioExistente;

                var problemDetails = new ProblemDetails();
                problemDetails.Extensions["errorMessage"] = ErrorCatalogo.GetMensajeError(codigoError);
                problemDetails.Extensions["errorComment"] = "¿Desea iniciar sesión?";
                problemDetails.Extensions["errorCode"] = codigoError;

                return BadRequest(problemDetails);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }




    }
}
