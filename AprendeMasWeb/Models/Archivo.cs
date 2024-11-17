namespace AprendeMasWeb.Models
{
	public class Archivo
	{
		public int ArchivoId { get; set; }
		public int ActividadId { get; set; }
		public Actividad Actividad { get; set; }
		public string NombreArchivo { get; set; }
		public string DescripcionArchivo { get; set; }
		public string URLArchivo { get; set; }
		public DateTime FechaSubida { get; set; }
	}
}
