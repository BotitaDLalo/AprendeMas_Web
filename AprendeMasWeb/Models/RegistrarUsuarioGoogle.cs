namespace AprendeMasWeb.Models
{
    public class RegistrarUsuarioGoogle
    {
        public required string IdToken { get; set; }
        public required string Role {  get; set; }
    }
}
