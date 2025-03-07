using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers.WEB
{
    public class MateriasController : Controller
    {
        // Acción para cargar las vistas parciales según la sección seleccionada
        public IActionResult CargarSeccion(string seccion)
        {
            if (string.IsNullOrEmpty(seccion))
            {
                return BadRequest("Sección no válida.");
            }

            return PartialView($"~/Views/Docente/MateriasDetallesParciales/_{seccion}.cshtml");
        }
    }
}
