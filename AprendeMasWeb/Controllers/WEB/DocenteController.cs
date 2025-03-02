// Se importan los espacios de nombres necesarios para trabajar con la base de datos, los modelos, Firebase y ASP.NET Core
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AprendeMasWeb.Controllers.WEB
{
    // Se usa el atributo Authorize para restringir el acceso al controlador solo a usuarios con el rol 'Docente'
    [Authorize(Roles = "Docente")]
    public class DocenteController : Controller
    {
        // Se declara el contexto de la base de datos para interactuar con los datos
        private readonly DataContext _context;

        // Acción que retorna la vista principal del docente
        public IActionResult Index()
        {
            return View(); // Retorna la vista 'Index' sin realizar ninguna lógica adicional
        }

        // Acción que retorna la vista del perfil del docente
        public IActionResult Perfil()
        {
            // Lógica para mostrar el perfil del docente (aunque no hay implementación específica aquí)
            return View(); // Retorna la vista 'Perfil'
        }

        // Acción HTTP POST para crear un nuevo aviso
        [HttpPost]
        public async Task<IActionResult> CrearAviso(Avisos aviso)
        {
            // Verifica si el modelo enviado es válido (por ejemplo, si todos los campos obligatorios están completos)
            if (ModelState.IsValid)
            {
                aviso.FechaCreacion = DateTime.Now; // Establece la fecha de creación del aviso

                //_context.Avisos.Add(aviso); // (Comentado) Código para agregar el aviso a la base de datos
                await _context.SaveChangesAsync(); // Guarda los cambios realizados en la base de datos

                // Se crea un mensaje para enviar una notificación a través de Firebase Cloud Messaging (FCM)
                var message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = aviso.Titulo, // Título de la notificación
                        Body = aviso.Descripcion // Descripción de la notificación
                    },
                    Topic = "avisos" // Se especifica el tópico al cual se enviará la notificación (en este caso, 'avisos')
                };

                // Enviar la notificación a través de Firebase
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

                // Imprime la respuesta de Firebase en la consola (para depuración)
                Console.WriteLine("Notificación enviada: " + response);

                // Redirige a la vista 'MateriasDetalles' después de enviar el aviso
                return RedirectToAction("MateriasDetalles");
            }
            return View(aviso); // Si el modelo no es válido, regresa la vista con el modelo actual para mostrar errores
        }

        // Acción para mostrar los detalles de una materia
        public IActionResult MateriasDetalles()
        {
            // Lógica para mostrar en vista cuando se accede a una materia (sin lógica específica en este caso)
            return View(); // Retorna la vista 'MateriasDetalles'
        }
    }
}
