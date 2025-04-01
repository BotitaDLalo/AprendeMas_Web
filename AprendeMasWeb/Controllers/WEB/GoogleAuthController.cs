using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AprendeMasWeb.Controllers.WEB
{
	public class GoogleAuthController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly DataContext _context;

		public GoogleAuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DataContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
		}

		[HttpGet]
		public IActionResult SignInWithGoogle()
		{
			var redirectUrl = Url.Action(nameof(GoogleResponse), "GoogleAuth");
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet]
		public async Task<IActionResult> GoogleResponse()
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return RedirectToAction("IniciarSesion", "Cuenta");
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			if (string.IsNullOrEmpty(email))
			{
				return RedirectToAction("IniciarSesion", "Cuenta");
			}

			var existingUser = await _userManager.FindByEmailAsync(email);

			if (existingUser == null)
			{
				// Guardamos el email en TempData para autocompletar el registro
				TempData["GoogleEmail"] = email;
				return RedirectToAction("ValidarCorreo", "Usuarios");
			}

			// Obtener los roles del usuario
			var roles = await _userManager.GetRolesAsync(existingUser);

			// Iniciar sesión
			await _signInManager.SignInAsync(existingUser, isPersistent: false);

			// Redirigir según el rol del usuario
			if (roles.Contains("Alumno"))
			{
				return RedirectToAction("Index", "Alumno");
			}
			else if (roles.Contains("Docente"))
			{
				return RedirectToAction("Index", "Docente");
			}
			else
			{
				// Si el usuario no tiene un rol específico, redirigirlo a una página general
				return RedirectToAction("IniciarSesion", "Cuenta");
			}
		}
	}
}
