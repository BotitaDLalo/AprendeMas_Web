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

namespace AprendeMasWeb.Controllers.Autenticacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager, DataContext context) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly DataContext _context = context;


        [HttpPost("RegistroUsuario")]
        public async Task<IActionResult> RegistroUsuario([FromBody] UsuarioRegistro modelo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var emailEncontrado = await _userManager.FindByEmailAsync(modelo.Correo);

                    if (emailEncontrado != null)
                    {
                        return BadRequest(new
                        {
                            mensaje = "El correo ya esta en uso"
                        });
                    }

                    var nombreUsuarioEncontrado = await _userManager.FindByNameAsync(modelo.NombreUsuario);

                    if (nombreUsuarioEncontrado != null)
                    {
                        return BadRequest(new
                        {
                            mensaje = "El nombre de usuario ya esta en uso"
                        });
                    }


                    var userName = modelo.NombreUsuario;
                    var email = modelo.Correo;
                    var rol = modelo.TipoUsuario;

                    var usuario = new IdentityUser()
                    {
                        UserName = userName,
                        Email = email,
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

                    if (modelo.TipoUsuario == "Docente")
                    {
                        var identityUserId = await _userManager.GetUserIdAsync(usuario);

                        Docentes docentes = new()
                        {
                            ApellidoPaterno = modelo.ApellidoPaterno,
                            ApellidoMaterno = modelo.ApellidoMaterno,
                            Nombre = modelo.Nombre,
                            UserId = identityUserId.ToString(),
                        };
                        _context.tbDocentes.Add(docentes);
                        _context.SaveChanges();
                    }
                    else if (modelo.TipoUsuario == "Alumno")
                    {
                        var identityUserId = await _userManager.GetUserIdAsync(usuario);
                        Alumnos alumnos = new()
                        {
                            ApellidoPaterno = modelo.ApellidoPaterno,
                            ApellidoMaterno = modelo.ApellidoMaterno,
                            Nombre = modelo.Nombre,
                            UserId = identityUserId,
                        };
                        _context.tbAlumnos.Add(alumnos);
                        _context.SaveChanges();
                    }


                    return Ok(new { nombre = userName, correo = email, rol });

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
                        ErrorCode = ErrorCatalogo.ErrorCodigos.UsuarioNoEncontrado,
                        ErrorMessage = ErrorCatalogo.GetMensajeError(ErrorCatalogo.ErrorCodigos.UsuarioNoEncontrado)
                    });
                }

                //Verificar password
                var user = await _signInManager.CheckPasswordSignInAsync(emailEncontrado, model.Clave, lockoutOnFailure: true);
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


                //Generando jwt
                var handler = new JwtSecurityTokenHandler();
                var confSecretKey = _configuration["jwt:SecretKey"];
                var jwt = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(confSecretKey ?? throw new ArgumentNullException(confSecretKey, "Token no configurado")));
                var credentials = new SigningCredentials(jwt, SecurityAlgorithms.HmacSha256);


                int idUsuario = 0;
                var identityUserId = await _userManager.GetUserIdAsync(emailEncontrado);

                if (rolUsuario == "Docente")
                {

                    idUsuario = _context.tbDocentes.Where(a => a.UserId == identityUserId).Select(a => a.DocenteId).FirstOrDefault();

                }
                else if (rolUsuario == "Alumno")
                {
                    idUsuario = _context.tbAlumnos.Where(a => a.UserId == identityUserId).Select(a => a.AlumnoId).FirstOrDefault();
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = "Aprende_Mas",
                    Audience = "Aprende_Mas",
                    SigningCredentials = credentials,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Subject = GenerarClaims(idUsuario, emailEncontrado, rolUsuario),
                };

                var token = handler.CreateToken(tokenDescriptor);

                var tokenString = handler.WriteToken(token);



                return Ok(new
                {
                    Id = idUsuario,
                    userName = emailEncontrado.UserName,
                    correo = emailEncontrado.Email,
                    rol = rolUsuario,
                    token = tokenString
                });
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

                return Ok(new { id = int.Parse(idUsuario), userName, correo, rol, token });
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

        private static ClaimsIdentity GenerarClaims(int idUsuario,IdentityUser usuario, string rol)
        {
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString() ?? ""));
            claims.AddClaim(new Claim(ClaimTypes.Name, usuario.UserName ?? ""));
            claims.AddClaim(new Claim(ClaimTypes.Email, usuario.Email ?? ""));
            claims.AddClaim(new Claim(ClaimTypes.Role, rol ?? ""));

            return claims;
        }


    }
}
