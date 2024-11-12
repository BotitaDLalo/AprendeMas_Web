using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AprendeMasWeb.Models
{
    public class GrupoRegistro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoId { get; set; }
        public String? NombreGrupo { get; set; }
        public String? Descripcion { get; set; }
        public String?  CodigoColor { get; set; }
        public String? CodigoAcceso { get; set; }
    }
}
