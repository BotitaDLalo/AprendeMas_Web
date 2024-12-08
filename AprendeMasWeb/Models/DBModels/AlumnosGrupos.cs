using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class AlumnosGrupos
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlumnoGrupoId { get; set; }

        public required int AlumnoId { get; set; }

        public virtual Alumnos? Alumnos { get; set; }

        public required int GrupoId { get; set; }

        public virtual Grupos? Grupos { get; set; }

    }
}
