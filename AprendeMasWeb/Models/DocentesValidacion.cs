namespace AprendeMasWeb.Models
{
    public class DocentesValidacion
    {
        public int DocenteId { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Nombre { get; set; }
        public required string Email { get; set; }
        public string? Autorizado { get; set; }
        public string? EnvioCorreo { get; set; }
    }
}
