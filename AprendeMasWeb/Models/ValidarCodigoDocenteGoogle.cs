namespace AprendeMasWeb.Models
{
    public class ValidarCodigoDocente
    {
        public required string Email { get; set; } 
        public required string CodigoValidar { get; set; }
        public string? IdToken { get; set; }
    }
}
