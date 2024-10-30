using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models
{
    public class Actividades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActividadId { get; set; }

        public required string NombreActividad { get; set; }

        public string? Descripcion { get; set; }

        public required DateTime FechaCreacion { get; set; }

        public int MateriaId { get; set; }
        [ForeignKey("MateriaId")]
        [JsonIgnore]
        public virtual MateriaRegistro? Materia { get; set; }

    }
}
