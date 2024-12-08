using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class Materias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MateriaId { get; set; }
        public required string NombreMateria { get; set; }
        public string? Descripcion { get; set; }

        public string? CodigoColor { get; set; }

        public string? CodigoAcceso { get; set; }

        public required int DocenteId { get; set; }


        public virtual Docentes? Docentes { get; set; }

        public virtual ICollection<GruposMaterias>? GruposMaterias { get; set; }

        public virtual ICollection<AlumnosMaterias>? AlumnosMaterias { get; set; }

        public  virtual ICollection<MateriasActividades>? MateriasActividades { get; set; }

        public  virtual ICollection<EventosMaterias>? EventosMaterias { get; set; }
    }
}
