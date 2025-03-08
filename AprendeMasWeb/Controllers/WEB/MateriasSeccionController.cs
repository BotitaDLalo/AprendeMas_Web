using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers.WEB
{
    public class MateriasSeccionController : Controller
    {
        // Acción para cargar las vistas parciales según la sección seleccionada
        public IActionResult CargarSeccion(string seccion)
        {
            Console.WriteLine($"Se ejecutó CargarSeccion con la sección: {seccion}");
            if (string.IsNullOrEmpty(seccion))
            {
                return BadRequest("Sección no válida.");
            }

            return PartialView($"~/Views/Docente/MateriasDetallesParciales/_{seccion}.cshtml");
        }
    }
}
