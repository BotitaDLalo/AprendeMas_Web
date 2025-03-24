using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Recursos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using AprendeMasWeb.Models;

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


        public IActionResult RegistrarUsuario(string email)
        {
                ViewBag.Email = email;
                return View();
        }

        public IActionResult ValidarCorreo()
        {
            return View();
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

            var email = HttpContext.Session.GetString(Recursos.SessionKeys.Email);


            if(email==null)
            {
                var codigoError = ErrorCatalogo.ErrorCodigos.emailNoValido;
                var problemDetails = new ProblemDetails();
                problemDetails.Extensions["errorMessage"] = ErrorCatalogo.GetMensajeError(codigoError);
                return BadRequest(problemDetails);
            }
                
            var nombre = usuario.Nombre;
            var apellidoPaterno = usuario.ApellidoPaterno;
            var apellidoMaterno = usuario.ApellidoMaterno;
            var password = usuario.Clave;
            var role = usuario.TipoUsuario;


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
            if (role == Recursos.Roles.ALUMNO)
            {
                tbAlumnos alumnos = new()
                {
                    ApellidoPaterno = apellidoPaterno,
                    ApellidoMaterno = apellidoMaterno,
                    Nombre = nombre,
                    UserId = identityUser.Id,
                };
                await _context.tbAlumnos.AddAsync(alumnos);

                await _context.SaveChangesAsync();
                return Ok(new AutenticacionRespuesta
                {
                    EstaAutorizado = EstatusAutorizacion.AUTORIZADO
                });
            }
            else if (role == Recursos.Roles.DOCENTE)
            {

                DateTime fechaExpiracionCodigo = DateTime.UtcNow.AddMinutes(59);
                string codigo = RecursosGenerales.GenerarCodigoAleatorio();
                tbDocentes docentes = new()
                {
                    ApellidoPaterno = apellidoPaterno,
                    ApellidoMaterno = apellidoMaterno,
                    Nombre = nombre,
                    UserId = identityUser.Id,
                    CodigoAutorizacion = codigo,
                    FechaExpiracionCodigo = fechaExpiracionCodigo,
                };
                _context.tbDocentes.Add(docentes);

                await _context.SaveChangesAsync();
                return Ok(new AutenticacionRespuesta
                {
                    EstaAutorizado = EstatusAutorizacion.PENDIENTE
                });
            }


            //return RedirectToAction("IniciarSesion", "Cuenta");

            return BadRequest();
        }
    }
}