namespace AprendeMasWeb.Models
{
	public class AlumnoActividad
	{
		public int AlumnoActividadId { get; set; }
		public int AlumnoId { get; set; }
		public Alumno Alumno { get; set; }
		public int ActividadId { get; set; }
		public Actividad Actividad { get; set; }
		public DateTime FechaEntrega { get; set; }
		public byte EstatusEntregada { get; set; }
	}
}
