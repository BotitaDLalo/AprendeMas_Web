using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{
    public class MateriaConGrupo
    {
        public required string NombreMateria { get; set; }
        public string? Descripcion { get; set; }

        public required int DocenteId { get; set; }
        //public string? CodigoColor { get; set; }
        public required List<int> Grupos { get; set; }
    }
}
