using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace AprendeMasWeb.Controllers.WEB
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CuentaController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }



        [HttpGet]
        public IActionResult IniciarSesion()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string email, string password)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Alumno"))
                    {
                        return RedirectToAction("Index", "Alumno"); // Redirige a la vista del alumno
                    }
                    else if (roles.Contains("Docente"))
                    {
                        return RedirectToAction("Index", "Docente"); // Redirige a la vista del docente
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("IniciarSesion", "Cuenta");
        }
    }
}