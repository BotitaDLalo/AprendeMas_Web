using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{
    public class GrupoMateriasRegistro
    {
        public required int DocenteId {  get; set; }
        public required string NombreGrupo { get; set; }
        public string? Descripcion { get; set; }
        //public required string CodigoColor { get; set; }
        public string? CodigoAcceso { get; set; }
        public required List<MateriasP> Materias { get; set; }
    }
}
