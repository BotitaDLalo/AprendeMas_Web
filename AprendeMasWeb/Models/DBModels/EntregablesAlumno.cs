using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class EntregablesAlumno
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int EntregaId {  get; set; }

        public required int AlumnoActividadId { get; set; }

        public string? Enlace {  get; set; }

        public string? Archivo {  get; set; }

        public AlumnosActividades? AlumnosActividades { get; set; }
    }
}
