using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AprendeMasWeb.Models.DBModels;


namespace AprendeMasWeb.Models.DBModels
{
    public class EventosMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int EventoMateriaId { get; set; }

        public required int FechaId { get; set; }

        public required int MateriaId {  get; set; }
        public virtual EventosAgenda? EventosAgenda { get; set; }
        public virtual Materias? Materias { get; set; }
    }
}
