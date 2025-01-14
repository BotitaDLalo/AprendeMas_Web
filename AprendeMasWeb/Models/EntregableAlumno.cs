namespace AprendeMasWeb.Models
{
    public class EntregableAlumno
    {
        public required int ActividadId {  get; set; }

        public required int AlumnoId { get; set; }

        public string? Respuesta {  get; set; }

        public required string FechaEntrega {  get; set; }

        public List<string>? Enlaces { get; set; }
    }
}
