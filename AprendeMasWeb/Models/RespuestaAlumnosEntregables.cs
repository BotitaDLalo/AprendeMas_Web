namespace AprendeMasWeb.Models
{
    public class RespuestaAlumnosEntregables
    {
        public int ActividadId { get; set; }
        public int Puntaje {  get; set; }
        public int TotalEntregados {  get; set; }
        public List<AlumnoEntregable>? AlumnosEntregables { get; set; }
    }


    public class AlumnoEntregable
    {
        public int EntregaId { get; set; }
        public int AlumnoId { get; set; }
        public string NombreUsuario {  get; set; }
        public string Nombres {  get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Respuesta { get; set; }
        public int Calificacion { get; set; }
    }
}
