using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbGrupos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GrupoId { get; set; }
        public required string NombreGrupo { get; set; }
        public string? Descripcion { get; set; }
        public string? CodigoAcceso { get; set; }
        public string? CodigoColor { get; set; }
        public required int DocenteId { get; set; }

        public virtual tbDocentes? Docentes { get; set; }
        public  ICollection<tbGruposMaterias>? GruposMaterias { get; set; }
        public ICollection<tbAlumnosGrupos>? AlumnosGrupos { get; set; }  

        public ICollection<tbEventosGrupos>? EventosGrupos { get; set; }
    }
}
