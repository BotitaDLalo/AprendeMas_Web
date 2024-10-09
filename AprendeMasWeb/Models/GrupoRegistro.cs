using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AprendeMasWeb.Models
{
    public class GrupoRegistro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoId { get; set; }
        public required string NombreGrupo { get; set; }
        public string? Descripcion { get; set; }
        public string CodigoAcceso { get; set; }
        public required string TipoUsuario { get; set; }

        public List<MateriaRegistro> Materias { get; set; } = new List<MateriaRegistro>();

    }
}
