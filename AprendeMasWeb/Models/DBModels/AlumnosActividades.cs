using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class AlumnosActividades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int AlumnoActividadId { get; set; }

        public required int ActividadId { get; set; }
        public Actividades? Actividades {  get; set; } 
        public required int AlumnoId { get; set; }
        public Alumnos? Alumnos { get; set; }

        public required DateTime FechaEntrega { get; set; }

        public required bool EstatusEntrega { get; set; } 

        public EntregablesAlumno? EntregablesAlumno { get; set; }

    }
}
