namespace AprendeMasWeb.Models
{
	public class Calificacion
	{
		public int CalificacionId { get; set; }
		public int AlumnoId { get; set; }
		public Alumno Alumno { get; set; }
		public int ActividadId { get; set; }
		public Actividad Actividad { get; set; }
		public int Valor { get; set; }
		public DateTime FechaCalificacionAsignada { get; set; }
		public string Comentarios { get; set; }
	}
}
