namespace AprendeMasWeb.Models
{
	public class Actividad
	{
		public int ActividadId { get; set; }
		public string Descripcion { get; set; }
		public int TipoActividadId { get; set; }
		public TipoActividad TipoActividad { get; set; }
		public int? RubricaId { get; set; }
		public RubricaEvaluacion Rubrica { get; set; }
		public DateTime FechaCreacion { get; set; }
		public DateTime FechaLimite { get; set; }
		public int? Puntuacion { get; set; }
		public ICollection<Tarea> Tareas { get; set; }
		public ICollection<Archivo> Archivos { get; set; }
		public ICollection<AlumnoActividad> AlumnoActividades { get; set; }
		public ICollection<Calificacion> Calificaciones { get; set; }
	}
}
