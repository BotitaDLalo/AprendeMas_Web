using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AprendeMasWeb.Models.DBModels;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbEventosGrupos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventoGrupoId { get; set; }

        public int? FechaId { get; set; }

        public required int GrupoId { get; set; }

        public virtual tbEventosAgenda? EventosAgenda { get; set; }
        public virtual tbGrupos? Grupos { get; set; }
    }
}
