namespace AprendeMasWeb.Models
{
    public class CancelarEnvioActividadAlumno
    {
        public required int AlumnoActividadId {  get; set; }

        public required int ActividadId {  get; set; }

        public required int AlumnoId { get; set; }
    }
}
