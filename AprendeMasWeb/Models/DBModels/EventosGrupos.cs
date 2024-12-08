using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AprendeMasWeb.Models.DBModels;

namespace AprendeMasWeb.Models.DBModels
{
    public class EventosGrupos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int EventoGrupoId { get; set; }

        public required int FechaId { get; set; }

        public required int GrupoId { get; set; }

        public virtual EventosAgenda? EventosAgenda { get; set; }
        public virtual Grupos? Grupos { get; set; }
    }
}
