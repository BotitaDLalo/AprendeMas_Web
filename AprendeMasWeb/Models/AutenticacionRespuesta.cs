using System.Text.Json.Serialization;


namespace AprendeMasWeb.Models
{
    public class AutenticacionRespuesta
    {
        public int Id { get; set; }
        
        public string? UserName { get; set; }

        public string? Correo { get; set; }
        
        public string? Rol { get; set; }

        public string? Token {  get; set; }
        
        public string? EstaAutorizado { get; set; }

        public bool? RequiereDatosAdicionales { get; set; }
    }
}
