using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbMaterias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MateriaId { get; set; }
        public required string NombreMateria { get; set; }
        public string? Descripcion { get; set; }

        public string? CodigoColor { get; set; }

        public string? CodigoAcceso { get; set; }

        public required int DocenteId { get; set; }


        public virtual tbDocentes? Docentes { get; set; }

        public virtual ICollection<tbGruposMaterias>? GruposMaterias { get; set; }

        public virtual ICollection<tbAlumnosMaterias>? AlumnosMaterias { get; set; }

        public virtual ICollection<tbActividades>? Actividades { get; set; }

        public  virtual ICollection<tbEventosMaterias>? EventosMaterias { get; set; }
    }
}
