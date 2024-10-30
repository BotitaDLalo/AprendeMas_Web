using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models
{
    public class GruposMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoMateriasId { get; set; }

        public int GrupoId { get; set; }
        [ForeignKey("GrupoId")]
        [JsonIgnore]
        public virtual GrupoRegistro? Grupo { get; set; }

        public int MateriaId { get; set; }
        [ForeignKey("MateriaId")]
        [JsonIgnore]
        public virtual MateriaRegistro? Materia { get; set; }
    }
}
