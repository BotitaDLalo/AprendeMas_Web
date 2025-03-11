using AprendeMasWeb.Data; // Agrega la referencia al contexto de datos de la aplicación
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
            if (!ModelState.IsValid) // Verifica si el modelo es válido (evita datos incorrectos o vacíos)
                return View();

            var user = await _userManager.FindByEmailAsync(email); // Busca el usuario en la base de datos por su email
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos."); // Mensaje de error si no encuentra el usuario
                return View();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false); // Verifica la contraseña
            if (result.Succeeded) // Si la contraseña es correcta
            {
                var claims = new List<Claim> // Lista de claims para almacenar información del usuario
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id) // Guarda el ID del usuario como claim
                };

                // Verifica si el usuario es un docente
                var docente = await _context.tbDocentes.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (docente != null)
                {
                    claims.Add(new Claim("DocenteId", docente.DocenteId.ToString())); // Agrega el ID del docente como claim
                }

                // Verifica si el usuario es un alumno
                var alumno = await _context.tbAlumnos.FirstOrDefaultAsync(a => a.UserId == user.Id);
                if (alumno != null)
                {
                    claims.Add(new Claim("AlumnoId", alumno.AlumnoId.ToString())); // Agrega el ID del alumno como claim
                }

                // Crea una identidad basada en los claims
                var claimsIdentity = new ClaimsIdentity(claims, "login");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Inicia sesión con los claims asignados
                await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

                // Redirige al usuario según su rol en el sistema
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Alumno"))
                {
                    return RedirectToAction("Index", "Alumno"); // Redirige a la vista de alumnos
                }
                else if (roles.Contains("Docente"))
                {
                    return RedirectToAction("Index", "Docente"); // Redirige a la vista de docentes
                }
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos."); // Mensaje de error si la autenticación falla
            return View(); // Retorna a la vista de inicio de sesión
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
