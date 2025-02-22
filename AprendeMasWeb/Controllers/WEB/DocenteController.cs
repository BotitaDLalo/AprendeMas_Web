using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Messaging;
using Google;
using AprendeMasWeb.Data;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Docente")] // Restringir acceso solo a usuarios con rol Docente
    public class DocenteController : Controller
    {
        private readonly DataContext _context;
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

        [HttpPost]
        public async Task<IActionResult> CrearAviso(Avisos aviso)
        {
            if (ModelState.IsValid)
            {
                aviso.FechaCreacion = DateTime.Now;
                //_context.Avisos.Add(aviso);
                await _context.SaveChangesAsync();

                // Enviar notificación a FCM
                var message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = aviso.Titulo,
                        Body = aviso.Descripcion
                    },
                    Topic = "avisos"
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Notificación enviada: " + response);

                return RedirectToAction("MateriasDetalles");
            }

            return View(aviso);

            
        }
    }
}