using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Services
{
    public interface ITiposActividadesService
    {
        Task<TiposActividades> GetOrCreateTipoActividad(int tipoActividadId);
    }

    public class TiposActividadesService : ITiposActividadesService
    {
        private readonly DataContext _context;

        public TiposActividadesService(DataContext context)
        {
            _context = context;
        }

        public async Task<TiposActividades> GetOrCreateTipoActividad(int tipoActividadId)
        {
            // Buscar si el tipo de actividad ya existe
            var tipoActividad = await _context.cTiposActividades
                .FirstOrDefaultAsync(t => t.TipoActividadId == tipoActividadId);

            // Si no existe, crearlo automáticamente
            if (tipoActividad == null)
            {
                string nombreTipoActividad = tipoActividadId switch
                {
                    1 => "Actividad",
                    2 => "Examen",
                    3 => "Archivo",
                    _ => throw new ArgumentException("Tipo de actividad no válido.")
                };

                tipoActividad = new TiposActividades
                {
                    TipoActividadId = tipoActividadId,
                    Nombre = nombreTipoActividad
                };

                _context.cTiposActividades.Add(tipoActividad);
                await _context.SaveChangesAsync();
            }

            return tipoActividad;
        }
    }

}
