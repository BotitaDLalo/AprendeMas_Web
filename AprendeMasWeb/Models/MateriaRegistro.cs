using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models
{
    public class MateriaRegistro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MateriaId { get; set; }
        public required string NombreMateria { get; set; }
        public string? Descripcion { get; set; }

        public int GrupoId { get; set; }


        [ForeignKey("GrupoId")]
        [JsonIgnore]
        public virtual GrupoRegistro Grupo { get; set; }
    }
}
