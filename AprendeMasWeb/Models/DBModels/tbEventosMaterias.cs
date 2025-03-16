using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AprendeMasWeb.Models.DBModels;


namespace AprendeMasWeb.Models.DBModels
{
    public class tbEventosMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventoMateriaId { get; set; }

        public int? FechaId { get; set; }

        public required int MateriaId {  get; set; }
        public virtual tbEventosAgenda? EventosAgenda { get; set; }
        public virtual tbMaterias? Materias { get; set; }
    }
}
