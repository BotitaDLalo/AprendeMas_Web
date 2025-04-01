using AprendeMasWeb.Data; // Agrega la referencia al contexto de datos de la aplicación
using AprendeMasWeb.Models;
using AprendeMasWeb.Recursos;
using Microsoft.AspNetCore.Identity; // Librería para la gestión de identidad y autenticación
using Microsoft.AspNetCore.Mvc; // Librería para los controladores en ASP.NET Core MVC
using Microsoft.EntityFrameworkCore; // Permite realizar consultas asíncronas en la base de datos
using System.Security.Claims; // Manejo de Claims para autenticación y autorización

namespace AprendeMasWeb.Controllers.WEB
{

    public class CuentaController : Controller // Controlador encargado de la autenticación de usuarios
    {
        private readonly SignInManager<IdentityUser> _signInManager; // Maneja el inicio y cierre de sesión
        private readonly UserManager<IdentityUser> _userManager; // Permite gestionar usuarios en la base de datos
        private readonly DataContext _context; // Contexto de la base de datos

        // Constructor del controlador que inyecta dependencias necesarias para la autenticación y acceso a la base de datos
        public CuentaController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, DataContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context; // Asigna el contexto de base de datos
        }
            
        // Acción HTTP GET para mostrar la vista de inicio de sesión
        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View(); // Devuelve la vista de inicio de sesión
        }

        // Acción HTTP POST para procesar el inicio de sesión
        [HttpPost]
		public async Task<IActionResult> IniciarSesion(string email, string password)
		{
			if (!ModelState.IsValid)
				return View();

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				TempData["ErrorMensaje"] = "El correo ingresado no existe.";
				return View();
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
			if (!result.Succeeded)
			{
				TempData["ErrorMensaje"] = "La contraseña es incorrecta.";
				return View();
			}

			
		
			
			if (result.Succeeded)
			{
				var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id)
		};

				// Verifica si el usuario es Administrador
				var administrador = await _context.tbAdministradores.FirstOrDefaultAsync(a => a.UserId == user.Id);
				if (administrador != null)
				{
					claims.Add(new Claim("AdministradorId", administrador.AdministradorId.ToString()));
				}

				// Verifica si el usuario es Docente
				var docente = await _context.tbDocentes.FirstOrDefaultAsync(d => d.UserId == user.Id);
				if (docente != null)
				{
					if (docente.estaAutorizado == null)
					{
						HttpContext.Session.SetString(Recursos.SessionKeys.Email, email);
						return RedirectToAction("VerificarCodigo", "Usuarios");
					}
					else if (!docente.estaAutorizado.Value)
					{
						return View();
					}
					else
					{
						claims.Add(new Claim("DocenteId", docente.DocenteId.ToString()));
					}
				}

				// Verifica si el usuario es Alumno
				var alumno = await _context.tbAlumnos.FirstOrDefaultAsync(a => a.UserId == user.Id);
				if (alumno != null)
				{
					claims.Add(new Claim("AlumnoId", alumno.AlumnoId.ToString()));
				}

				// Crea una identidad con los claims asignados
				var claimsIdentity = new ClaimsIdentity(claims, "login");
				var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

				// Inicia sesión con los claims asignados
				await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

				// Redirige según el rol
				var roles = await _userManager.GetRolesAsync(user);
				if (roles.Contains("Administrador"))
				{
					return RedirectToAction("Index", "Administrador");
				}
				else if (roles.Contains("Docente"))
				{
					return RedirectToAction("Index", "Docente");
				}
				else if (roles.Contains("Alumno"))
				{
					return RedirectToAction("Index", "Alumno");
				}
			}

			ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
			return View();
		}



		[HttpPost]
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

                    if (docente.FechaExpiracionCodigo < DateTime.Now)
                    {
                        return BadRequest(new { ErrorCode = ErrorCatalogo.ErrorCodigos.codigoAutorizacionExpirado });
                    }

                    docente.FechaExpiracionCodigo = null;
                    docente.CodigoAutorizacion = null;
                    docente.estaAutorizado = true;
                    _context.SaveChanges();

                    var claims = new List<Claim> // Lista de claims para almacenar información del usuario
                {
                    new Claim(ClaimTypes.NameIdentifier, identityUserId) // Guarda el ID del usuario como claim
                };
                    claims.Add(new Claim("DocenteId", docente.DocenteId.ToString()));

                    // Crea una identidad basada en los claims
                    var claimsIdentity = new ClaimsIdentity(claims, "login");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Inicia sesión con los claims asignados
                    await _signInManager.SignInWithClaimsAsync(emailEncontrado, isPersistent: false, claims);

                    var redirectUrl = Url.Action("Index", "Docente");
                    return Ok(new { redirectUrl = redirectUrl });
                }

                return BadRequest(new { mensaje = "El docente no existe." });
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }



        // Acción HTTP GET para obtener el ID del docente autenticado
        [HttpGet]
        public IActionResult ObtenerDocenteId()
        {
            var docenteId = User.FindFirstValue("DocenteId"); // Busca el claim con el ID del docente
            if (string.IsNullOrEmpty(docenteId))
            {
                return Json(new { error = "No se encontró el DocenteId" }); // Devuelve un error si no encuentra el claim
            }
            return Json(new { docenteId }); // Devuelve el ID del docente en formato JSON
        }

        [HttpGet]
        public IActionResult ObtenerAlumnoId()
        {
            var alumnoId = User.FindFirstValue("AlumnoId"); // Busca el claim con el ID del alumno
            if (string.IsNullOrEmpty(alumnoId))
            {
                return Json(new { error = "No se encontró el AlumnoId" }); // Devuelve un error si no encuentra el claim
            }
            return Json(new { alumnoId }); // Devuelve el ID del alumno en formato JSON
        }


        // Acción HTTP GET para verificar los claims almacenados en el usuario autenticado
        [HttpGet]
        public IActionResult VerificarClaims()
        {
            if (!User.Identity.IsAuthenticated) // Verifica si el usuario está autenticado
            {
                return Unauthorized("El usuario no está autenticado."); // Devuelve un error si no está autenticado
            }

            var claims = HttpContext.User.Claims // Obtiene todos los claims del usuario autenticado
                .Select(c => new { c.Type, c.Value }) // Selecciona el tipo y el valor de cada claim
                .ToList();

            return Ok(claims); // Devuelve la lista de claims en formato JSON
        }

        // Acción HTTP POST para cerrar sesión de manera segura
        [HttpPost]
        [ValidateAntiForgeryToken] // Previene ataques CSRF (Cross-Site Request Forgery)
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync(); // Cierra la sesión del usuario
            return RedirectToAction("IniciarSesion", "Cuenta"); // Redirige a la vista de inicio de sesión
        }

		[HttpGet]
		public IActionResult IniciarSesionGoogle()
		{
			var redirectUrl = Url.Action("GoogleResponse", "Cuenta");
			var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
			return Challenge(properties, "Google");
		}

		[HttpGet]
		public async Task<IActionResult> GoogleResponse()
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return RedirectToAction("IniciarSesion");
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);

			if (string.IsNullOrEmpty(email))
			{
				return RedirectToAction("IniciarSesion"); // Manejo de error si no se obtiene el correo.
			}

			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
                
				TempData["GoogleEmail"] = email;
				return RedirectToAction("Registrar", "Usuarios"); // Redirigir a la vista de registro
			}

			// Si el usuario ya existe, redirigir al login con el correo llenado automáticamente
			return RedirectToAction("IniciarSesion", new { email = email });
		}


	}
}
