using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbCalificaciones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CalificacionId { get; set; }

        public required int EntregaId { get; set; }

        public required DateTime FechaCalificacionAsignada {  get; set; }

        public string? Comentarios {  get; set; }
        public required int Calificacion { get; set; }
        public virtual tbEntregablesAlumno? EntregablesAlumno { get; set; }
    }
}
