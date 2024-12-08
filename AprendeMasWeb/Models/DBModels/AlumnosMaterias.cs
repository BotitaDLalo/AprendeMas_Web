using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class AlumnosMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlumnoMateriaId { get; set; }
        public required int AlumnoId { get; set; }
        public virtual Alumnos? Alumnos { get; set; }
        public required virtual int MateriaId { get; set; }
        public virtual Materias? Materias { get; set; }
    }
}
