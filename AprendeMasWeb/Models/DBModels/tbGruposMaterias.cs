using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbGruposMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoMateriasId { get; set; }

        public int GrupoId { get; set; }

        public virtual tbGrupos? Grupos { get; set; }

        public int MateriaId { get; set; }

        public virtual tbMaterias? Materias { get; set; }
    }
}
