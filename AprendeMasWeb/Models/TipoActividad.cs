

namespace AprendeMasWeb.Models
{
	public class TipoActividad
	{
		public int TipoActividadId { get; set; }
		public string NombreTipo { get; set; }
		public ICollection<Actividad> Actividades { get; set; }
	}
}
