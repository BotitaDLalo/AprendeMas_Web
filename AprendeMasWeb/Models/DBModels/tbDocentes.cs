using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbDocentes
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
        public virtual ICollection<tbGrupos>? Grupos { get; set; }
        public virtual ICollection<tbMaterias>? Materias { get; set; }
        public virtual ICollection<tbEventosAgenda>? EventosAgendas { get; set; }
        public virtual ICollection<tbAvisos>? Avisos { get; set; }
    }
}
