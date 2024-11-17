

using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{
	public class RubricaEvaluacion
	{
		[Key] // Clave primaria
		public int RubricaId { get; set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public float Porcentaje { get; set; }
		public ICollection<Actividad> Actividades { get; set; }
		public ICollection<Examen> Examenes { get; set; }
	}
}
