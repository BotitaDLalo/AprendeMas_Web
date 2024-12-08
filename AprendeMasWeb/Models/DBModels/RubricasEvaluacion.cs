using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class RubricasEvaluacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int RubricaId { get; set; }

        public required string Nombre { get; set; }

        public required string Descripcion { get; set; }
    }
}
