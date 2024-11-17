using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{
	public class EventoAgenda
	{
		[Key] // Clave primaria
		public int FechaId { get; set; }
		public int DocenteId { get; set; }
		public string NombreEvento { get; set; }
		public string ColorEvento { get; set; }
		public DateTime FechaMarcada { get; set; }
		public string Descripcion { get; set; }
	}
}
