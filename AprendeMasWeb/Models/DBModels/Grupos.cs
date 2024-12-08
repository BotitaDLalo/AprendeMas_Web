using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class Grupos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoId { get; set; }
        public required string NombreGrupo { get; set; }
        public string? Descripcion { get; set; }
        public string? CodigoAcceso { get; set; }
        public string? CodigoColor { get; set; }
        public required int DocenteId { get; set; }

        public virtual Docentes? Docentes { get; set; }
        public  ICollection<GruposMaterias>? GruposMaterias { get; set; }
        public ICollection<AlumnosGrupos>? AlumnosGrupos { get; set; }  

        public ICollection<EventosGrupos>? EventosGrupos { get; set; }
    }
}
