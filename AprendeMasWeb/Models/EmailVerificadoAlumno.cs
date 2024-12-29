namespace AprendeMasWeb.Models
{
    public class EmailVerificadoAlumno
    {
        public required string Email {  get; set; }
        
        public string? UserName {  get; set; }

        public string? Nombre { get; set; }

        public string? ApellidoPaterno { get; set; }

        public string? ApellidoMaterno { get; set; }
    }
}
