using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AprendeMasWeb.Models.DBModels
{
    public class Alumnos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  int AlumnoId { get; set; }

        public required string ApellidoPaterno { get; set; }

        public required string ApellidoMaterno { get; set; }

        public required string Nombre { get; set; }

        public virtual IdentityUser? IdentityUser { get; set; }
        
        [ForeignKey("IdentityUser")]
        public required string UserId { get; set; }
        public virtual ICollection<AlumnosTokens>? AlumnosTokens {  get; set; } 

        public virtual ICollection<AlumnosGrupos>? AlumnosGrupos { get; set; }
        
        public virtual ICollection<AlumnosMaterias>? AlumnosMaterias { get; set; }
        
        public virtual ICollection<AlumnosActividades>? AlumnosActividades { get; set; }
    }
}
