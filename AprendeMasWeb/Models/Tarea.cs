namespace AprendeMasWeb.Models
{
	public class Tarea
	{
		public int TareaId { get; set; }
		public int ActividadId { get; set; }
		public Actividad Actividad { get; set; }
		public string Instrucciones { get; set; }
		public DateTime FechaEntrega { get; set; }
	}
}
