namespace AprendeMasWeb.Models
{
	public class Notificacion
	{
		public int NotificacionId { get; set; }
		public string UsuarioId { get; set; }
		public string TipoNotificacion { get; set; }
		public int ReferenciaId { get; set; }
		public DateTime FechaNotificacion { get; set; }
		public bool Leido { get; set; }
	}
}
