using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Docente")] // Restringir acceso solo a usuarios con rol Docente
    public class DocenteController : Controller
    {
        public IActionResult Index()
        {
            //Sin funcionar
            // obtener el DocenteID del claim
            var docenteIdClaim = User.FindFirst("DocenteId"); //Usar el nombre correcto del claim
            if (docenteIdClaim != null)
            {
                ViewData["DocenteId"] = docenteIdClaim.Value;
            }
            else
            {
                ViewData["DocenteId"] = "No Encontrado";
            }
            return View();
        }

        public IActionResult Perfil()
        {
            // Lógica para mostrar el perfil del docente
            return View();
        }

        public IActionResult MateriasDetalles()
        {
            // Lógica para mostrar en vista cuando se accede a una materia. Nada de logica. para logica y consultas sera controlador api
            return View();
        }
    }
}