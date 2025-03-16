using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbAlumnosActividades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlumnoActividadId { get; set; }

        public required int ActividadId { get; set; }

        public required int AlumnoId { get; set; }

        public required DateTime FechaEntrega { get; set; }

        public required bool EstatusEntrega { get; set; } 

        public tbActividades? Actividades {  get; set; } 
        public tbAlumnos? Alumnos { get; set; }
        public tbEntregablesAlumno? EntregablesAlumno { get; set; }

    }
}
