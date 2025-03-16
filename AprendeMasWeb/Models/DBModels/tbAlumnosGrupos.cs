using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbAlumnosGrupos
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlumnoGrupoId { get; set; }

        public required int AlumnoId { get; set; }

        public virtual tbAlumnos? Alumnos { get; set; }

        public required int GrupoId { get; set; }

        public virtual tbGrupos? Grupos { get; set; }

    }
}
