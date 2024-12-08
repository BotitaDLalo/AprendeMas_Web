using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class Tareas
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int TareaId { get; set; }

        public required int ActividadId { get; set; }

        public required string Instrucciones { get; set; }

        public DateTime FechaEntrega { get; set; }
    }
}
