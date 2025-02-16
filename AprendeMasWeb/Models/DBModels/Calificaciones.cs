using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class Calificaciones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CalificacionId { get; set; }

        //public required int ActividadId { get; set; }

        //public virtual Actividades? Actividades { get; set; }

        public required int EntregaId { get; set; }

        public required DateTime FechaCalificacionAsignada {  get; set; }

        public string? Comentarios {  get; set; }
        public required int Calificacion { get; set; }
        public virtual EntregablesAlumno? EntregablesAlumno { get; set; }
    }
}
