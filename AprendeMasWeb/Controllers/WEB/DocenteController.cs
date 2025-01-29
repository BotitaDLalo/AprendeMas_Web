using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Docente")] // Restringir acceso solo a usuarios con rol Docente
    public class DocenteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            // Lógica para mostrar el perfil del docente
            return View();
        }

        public IActionResult Materias()
        {
            // Lógica para mostrar las materias del docente
            return View();
        }
    }
}