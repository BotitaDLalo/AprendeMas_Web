using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbAlumnosMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlumnoMateriaId { get; set; }
        public required int AlumnoId { get; set; }
        public virtual tbAlumnos? Alumnos { get; set; }
        public required virtual int MateriaId { get; set; }
        public virtual tbMaterias? Materias { get; set; }
    }
}
