using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbAlumnos
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

        public virtual ICollection<tbAlumnosGrupos>? AlumnosGrupos { get; set; }
        
        public virtual ICollection<tbAlumnosMaterias>? AlumnosMaterias { get; set; }
        
        public virtual ICollection<tbAlumnosActividades>? AlumnosActividades { get; set; }
    }
}
