namespace AprendeMasWeb.Models
{
	public class Alumno
	{
		public int AlumnoId { get; set; }
		public ICollection<AlumnoActividad> AlumnoActividades { get; set; }
		public ICollection<Calificacion> Calificaciones { get; set; }
	}
}
