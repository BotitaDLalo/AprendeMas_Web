using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Alumno")] // Restringir acceso solo a usuarios con rol Alumno
    public class AlumnoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            // Lógica para mostrar el perfil del alumno
            return View();
        }

        public IActionResult Actividades()
        {
            // Lógica para mostrar las actividades del alumno
            return View();
        }

        public IActionResult Materia()
        {
            // Lógica para mostrar las actividades del alumno
            return View();
        }
    }
}