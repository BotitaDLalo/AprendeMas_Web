using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Messaging;
using Google;
using AprendeMasWeb.Data;
using System.Security.Claims;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Docente")] // Restringir acceso solo a usuarios con rol Docente
    public class DocenteController : Controller
    {
        private readonly DataContext _context;
        public IActionResult Index()
        {
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

        public IActionResult MateriasDetalles()
        {
            // Lógica para mostrar en vista cuando se accede a una materia. Nada de logica. para logica y consultas sera controlador api
            return View();
        }


    }
}