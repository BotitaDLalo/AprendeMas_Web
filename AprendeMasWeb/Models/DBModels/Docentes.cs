using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AprendeMasWeb.Models.DBModels
{
    public class Docentes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocenteId { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Nombre { get; set; }
        public bool? estaAutorizado { get; set; }
        public bool? seEnvioCorreo {  get; set; }
        public DateTime? FechaExpiracionCodigo { get; set; }
        public string? CodigoAutorizacion {  get; set; }

        public virtual IdentityUser? IdentityUser { get; set; }
        [ForeignKey("IdentityUser")]
        public required string UserId { get; set; }
        public virtual ICollection<Grupos>? Grupos { get; set; }
        public virtual ICollection<Materias>? Materias { get; set; }
        public virtual ICollection<EventosAgenda>? EventosAgendas { get; set; }
        public virtual ICollection<Avisos>? Avisos { get; set; }
    }
}
