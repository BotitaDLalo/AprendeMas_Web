using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AprendeMasWeb.Controllers.WEB
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        public UsuariosController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

		[HttpGet]
		public IActionResult Registrar()
		{
			ViewBag.GoogleEmail = TempData["GoogleEmail"] as string;
			return View();
		}


		[HttpPost]
        public async Task<IActionResult> Registrar(string nombre, string apellidoPaterno, string apellidoMaterno, string email, string password, string role)
        {
            if (!ModelState.IsValid)
                return View();

            // Crear IdentityUser
            var identityUser = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(identityUser, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }

            // Asignar rol
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(identityUser, role);

            // Crear registro en la tabla correspondiente
            if (role == "Alumno")
            {
                var alumno = new Alumnos
                {
                    Nombre = nombre,
                    ApellidoPaterno = apellidoPaterno,
                    ApellidoMaterno = apellidoMaterno,
                    UserId = identityUser.Id
                };
                _context.tbAlumnos.Add(alumno);
            }
            else if (role == "Docente")
            {
                var docente = new Docentes
                {
                    Nombre = nombre,
                    ApellidoPaterno = apellidoPaterno,
                    ApellidoMaterno = apellidoMaterno,
                    UserId = identityUser.Id
                };
                _context.tbDocentes.Add(docente);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("IniciarSesion", "Cuenta");
        }
    }
}