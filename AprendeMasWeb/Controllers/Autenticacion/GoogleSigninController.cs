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

namespace AprendeMasWeb.Controllers.Autenticacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSigninController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DataContext context) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly DataContext _context = context;

        [HttpPost("RegistrarDatosFaltantesGoogle")]
        public async Task<IActionResult> RegistrarDatosFaltantesGoogle([FromBody] DatosFaltantesGoogle datos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int idUsuario = 0;
                    var nombres = datos.Nombres;
                    var apellidoPaterno = datos.ApellidoPaterno;
                    var apellidoMaterno = datos.ApellidoMaterno;
                    var role = datos.Role;
                    var token = datos.IdToken;


                    var payload = await GoogleJsonWebSignature.ValidateAsync(datos.IdToken);
                    if (payload == null)
                    {
                        return BadRequest("idToken invalido");
                    }


                    var email = payload.Email;
                    var userName = payload.GivenName;

                    var user = await _userManager.FindByEmailAsync(email);

                    if (user != null)
                    {
                        if (!await _roleManager.RoleExistsAsync(role))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(role));
                        }

                        var asignarRol = await _userManager.AddToRoleAsync(user, role);
                        if (!asignarRol.Succeeded)
                        {
                            return BadRequest(asignarRol.Errors);
                        }
                        if (role == "Docente")
                        {
                            var identityUserId = await _userManager.GetUserIdAsync(user);
                            Docentes docente = new()
                            {
                                Nombre = nombres,
                                ApellidoPaterno = apellidoPaterno,
                                ApellidoMaterno = apellidoMaterno,
                                UserId = identityUserId
                            };

                            _context.tbDocentes.Add(docente);
                            await _context.SaveChangesAsync();
                            idUsuario = _context.tbDocentes.Where(a => a.UserId == identityUserId).Select(a => a.DocenteId).FirstOrDefault();
                        }
                        else if (role == "Alumno")
                        {
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
                        }

                        var rol = await _userManager.GetRolesAsync(user);
                        var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");


                        return Ok(new 
                        { 
                            Id = idUsuario, 
                            userName, 
                            correo = email, 
                            rol = rolUsuario, 
                            token = token 
                        });
                    }
                    return BadRequest();
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }

        //[HttpPost("IniciarSesionGoogle")]
        //public async Task<IActionResult> IniciarSesionGoogle([FromBody] RegistrarUsuarioGoogle usuario)
        //{
        //    try
        //    {
        //        int idUsuario = 0;
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(usuario.IdToken);

        //        if (payload == null)
        //        {
        //            return BadRequest("idToken invalido");
        //        }

        //        var userName = payload.GivenName;
        //        var Email = payload.Email;
        //        var Token = usuario.IdToken;

        //        var emailEncontrado = await _userManager.FindByEmailAsync(Email);
        //        if (emailEncontrado != null)
        //        {
        //            var rol = await _userManager.GetRolesAsync(emailEncontrado);
        //            if (rol == null)
        //            {
        //                return Ok(new
        //                {
        //                    userName = userName,
        //                    correo = Email,
        //                    token = Token
        //                });
        //            }
        //            var identityUserId = await _userManager.GetUserIdAsync(emailEncontrado);
        //            var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");
        //            if (rolUsuario == "Docente")
        //            {
        //                idUsuario = _context.tbDocentes.Where(a => a.UserId == identityUserId).Select(a => a.DocenteId).FirstOrDefault();
        //            } else if (rolUsuario == "Alumno")
        //            {
        //                idUsuario = _context.tbAlumnos.Where(a => a.UserId == identityUserId).Select(a => a.AlumnoId).FirstOrDefault();
        //            }

        //            return Ok(new
        //            {
        //                Id = idUsuario,
        //                userName = userName,
        //                correo = Email,
        //                rol = rolUsuario,
        //                token = Token
        //            });

        //        }

        //        return Ok();

        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(new { mensaje = e.Message });
        //    }
        //}

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

                var userName = payload.GivenName;
                var Email = payload.Email;
                var Token = usuario.IdToken;

                var user = await _userManager.FindByEmailAsync(Email);

                if (user != null)
                {
                    //RETORNAR SIMPLEMENTE LOS DATOS COMO LOGIN NORMAL
                    var rol = await _userManager.GetRolesAsync(user);
                    if (rol.Count == 0)
                    {
                        return Ok(new
                        {
                            userName,
                            correo = Email,
                            token = Token
                        });
                    }
                    int idUsuario = 0;
                    var identityUserId = await _userManager.GetUserIdAsync(user);
                    var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");
                    if (rolUsuario == "Docente")
                    {
                        idUsuario = _context.tbDocentes.Where(a => a.UserId == identityUserId).Select(a => a.DocenteId).FirstOrDefault();
                    }
                    else if (rolUsuario == "Alumno")
                    {
                        idUsuario = _context.tbAlumnos.Where(a => a.UserId == identityUserId).Select(a => a.AlumnoId).FirstOrDefault();
                    }

                    return Ok(new
                    {
                        Id = idUsuario,
                        userName,
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
                    return BadRequest("Error creando usuario");
                }


                return Ok(new { userName, correo = Email, token = Token });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        //[HttpPost("IniciarSesionGoogle")]
        //public async Task<IActionResult> IniciarSesionGoogle([FromBody] RegistrarUsuarioGoogle usuario)
        //{
        //    try
        //    {
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(usuario.IdToken);

        //        if (payload == null)
        //        {
        //            return BadRequest("idToken invalido");
        //        }

        //        var userName = payload.GivenName;
        //        var Email = payload.Email;
        //        var Token = usuario.IdToken;

        //        var user = await _userManager.FindByEmailAsync(Email);
        //        if (user != null)
        //        {
        //            var rol = await _userManager.GetRolesAsync(user);
        //            if (rol == null)
        //            {
        //                return Ok(new
        //                {
        //                    nombre = userName,
        //                    correo = Email,
        //                    token = Token
        //                });
        //            }

        //            var rolUsuario = rol.FirstOrDefault() ?? throw new Exception("El usuario no posee un rol asignado");

        //            return Ok(new
        //            {
        //                nombre = userName,
        //                correo = Email,
        //                rol = rolUsuario,
        //                token = Token
        //            });
        //        }

        //        //return BadRequest();

        //        user = new IdentityUser
        //        {
        //            UserName = userName,
        //            Email = Email
        //        };


        //        var result = await _userManager.CreateAsync(user);

        //        if (!result.Succeeded)
        //        {
        //            return BadRequest("Error creating user.");
        //        }

        //        if (!await _roleManager.RoleExistsAsync(usuario.Role))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(usuario.Role));
        //        }

        //        var asignarRol = await _userManager.AddToRoleAsync(user, usuario.Role);
        //        if (!asignarRol.Succeeded)
        //        {
        //            return BadRequest(asignarRol.Errors);
        //        }

        //        return Ok(new
        //        {
        //            nombre = userName,
        //            correo = Email,
        //            rol = usuario.Role,
        //            token = Token
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest(new { mensaje = usuario.IdToken });
        //    }
        //}

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
                       idUsuario = _context.tbDocentes.Where(a => a.UserId == identityUserId).Select(a=>a.DocenteId).FirstOrDefault();
                    }else if (rolUsuario == "Alumno")
                    {
                        idUsuario = _context.tbAlumnos.Where(a => a.UserId == identityUserId).Select(a => a.AlumnoId).FirstOrDefault();
                    }

                    return Ok(new { Id = idUsuario ,userName = Name, correo = Email, rol = rolUsuario, token = Token });
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
