using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class GruposMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoMateriasId { get; set; }

        public int GrupoId { get; set; }

        public virtual Grupos? Grupos { get; set; }

        public int MateriaId { get; set; }

        public virtual Materias? Materias { get; set; }
    }
}
