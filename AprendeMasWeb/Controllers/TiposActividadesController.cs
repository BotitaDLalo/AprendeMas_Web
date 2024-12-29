//using AprendeMasWeb.Data;
//using AprendeMasWeb.Models.DBModels;
//using Google;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace AprendeMasWeb.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class TiposActividadesController : ControllerBase
//    {
//        private readonly DataContext _context;

//        public TiposActividadesController(DataContext context)
//        {
//            _context = context;
//        }

//        // Método para validar o crear el tipo de actividad
//        [NonAction] // Esto indica que no será un endpoint directamente invocable
//        public async Task<TiposActividades> GetOrCreateTipoActividad(int tipoActividadId)
//        {
//            // Buscar si el tipo de actividad ya existe
//            var tipoActividad = await _context.cTiposActividades
//                .FirstOrDefaultAsync(t => t.TipoActividadId == tipoActividadId);

//            // Si no existe, crearlo automáticamente
//            if (tipoActividad == null)
//            {
//                string nombreTipoActividad = tipoActividadId switch
//                {
//                    1 => "Actividad",
//                    2 => "Examen",
//                    3 => "Archivo",
//                    _ => throw new ArgumentException("Tipo de actividad no válido.")
//                };

//                tipoActividad = new TiposActividades
//                {
//                    TipoActividadId = tipoActividadId,
//                    Nombre = nombreTipoActividad
//                };

//                _context.cTiposActividades.Add(tipoActividad);
//                await _context.SaveChangesAsync();
//            }

//            return tipoActividad;
//        }
//    }

//}
