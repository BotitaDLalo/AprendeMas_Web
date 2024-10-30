using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AprendeMasWeb.Models
{
    public class GrupoRegistro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoId { get; set; }
        public string? NombreGrupo { get; set; }
        public string? Descripcion { get; set; }
        public  string? CodigoColor { get; set; }
        public string? CodigoAcceso { get; set; }
        //public string? TipoUsuario { get; set; }

    }
}
