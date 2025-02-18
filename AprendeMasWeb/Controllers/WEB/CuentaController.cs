using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using AprendeMasWeb.Data; //Agregar referencia al contexto
using AprendeMasWeb.Models.DBModels; //Se agrega referencia a los modelos
using Microsoft.EntityFrameworkCore; // Agregar para consultas async con _context

namespace AprendeMasWeb.Controllers.WEB
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DataContext _context; //Agregar context

        public CuentaController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, DataContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context; //Asignar context
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
                    //Buscar al docente en la base de datos
                    var docente = await _context.tbDocentes.FirstOrDefaultAsync(d => d.UserId == user.Id);

                    if (docente != null)
                    {
                        List<Claim> claims = new List<Claim>()
                        {
                            new Claim("DocenteId",docente.DocenteId.ToString()) //Aqui se guarda el DocenteId en claim para ser utilizado en diferentes controllers
                        };

                        //Crear un claimsidentity con los claims
                        var claimsIdentity = new ClaimsIdentity(claims, "login");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        //signIn el usuario con los claims adicionales
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        //Agregar los claims al usuario autenticado
                        HttpContext.User = claimsPrincipal;
                    }

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
