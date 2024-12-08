using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class EventosAgenda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int EventoId { get; set; }

        public required int DocenteId { get; set; }

        public virtual Docentes? Docentes { get; set; }

        public required DateTime FechaMarcada {  get; set; }

        public required string Descripcion { get; set; }

        public virtual ICollection<EventosGrupos>? EventosGrupos { get; set; }

        public virtual ICollection<EventosMaterias>? EventosMaterias { get; set; }
    }
}
