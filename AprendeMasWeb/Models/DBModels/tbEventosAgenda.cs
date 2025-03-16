using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbEventosAgenda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventoId { get; set; }

        public required int DocenteId { get; set; }

        public virtual tbDocentes? Docentes { get; set; }

        public required DateTime FechaInicio { get; set; }

        public required DateTime FechaFinal { get; set; }
        
        public required string Titulo { get; set; }

        public required string Descripcion { get; set; }

        public required string Color { get; set; }

        public virtual ICollection<tbEventosGrupos>? EventosGrupos { get; set; }

        public virtual ICollection<tbEventosMaterias>? EventosMaterias { get; set; }
    }
}
