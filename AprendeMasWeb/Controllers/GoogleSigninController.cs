using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSigninController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;


        [HttpPost("IniciarSesionGoogle")]
        public async Task<IActionResult> RegistrarUsuarioGoogle([FromBody] RegistrarUsuarioGoogle usuario)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(usuario.IdToken);

                if (payload == null)
                {
                    return BadRequest("idToken invalido");
                }

                var userName = payload.GivenName + payload.GivenName;
                var Email = payload.Email;
                var Token = usuario.IdToken;

                var user = await _userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    var rol = await _userManager.GetRolesAsync(user);
                    var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");

                    return Ok(new
                    {
                        nombre = userName,
                        correo = Email,
                        rol = rolUsuario,
                        token = Token
                    });
                }

                user = new IdentityUser
                {
                    UserName = userName,
                    Email = Email
                };


                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest("Error creating user.");
                }

                if (!await _roleManager.RoleExistsAsync(usuario.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(usuario.Role));
                }

                var asignarRol = await _userManager.AddToRoleAsync(user, usuario.Role);
                if (!asignarRol.Succeeded)
                {
                    return BadRequest(asignarRol.Errors);
                }

                return Ok(new
                {
                    nombre = userName,
                    correo = Email,
                    rol = usuario.Role,
                    token = Token
                });
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = usuario.IdToken });
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

                    var rol = await _userManager.GetRolesAsync(user);
                    var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");


                    return Ok(new { nombre = Name, correo = Email, rol = rolUsuario, token = Token });
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
