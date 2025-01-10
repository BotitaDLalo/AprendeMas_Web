namespace AprendeMasWeb.Models
{
    public class DatosFaltantesGoogle
    {
        /*Del usuario*/
        public required string Nombres { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno {  get; set; }
        public required string Role { get; set; }

        /*De google*/
        public required string IdToken { get; set; }
        //public required string NombreUsuario {  get; set; }
        //public required string Email { get; set; }

    }
}
