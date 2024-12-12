using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class MateriasActividades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MateriaActividad { get; set; }

        public required int MateriaId { get; set; }
        
        public virtual Materias? Materias { get; set; }

        public required int ActividadId { get; set; }
  
        public virtual Actividades? Actividades { get; set; }
    }
}
