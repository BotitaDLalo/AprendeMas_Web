using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{
    public class TipoUsuario
    {
        [Key]
        public int TipoUsuarioId { get; set; }
        public required string Usuario { get; set; }
    }
}
