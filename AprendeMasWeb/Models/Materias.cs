using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{
    public class Materias
    {
        public required string NombreMateria { get; set; }
        public string? Descripcion { get; set; }
        //public required string CodigoColor { get; set; }
    }
}
