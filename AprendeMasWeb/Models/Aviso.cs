namespace AprendeMasWeb.Models
{
	public class Aviso
	{
		public int AvisoId { get; set; }
		public string Asunto { get; set; }
		public string Descripcion { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime FechaFin { get; set; }
		public byte Estatus { get; set; }
	}
}
