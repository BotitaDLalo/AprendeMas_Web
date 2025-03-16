using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class tbAvisos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int  AvisoId { get; set; }

        public required int DocenteId { get; set; }

        public required string Titulo { get; set; }

        public required string Descripcion {  get; set; }

        public int? GrupoId { get; set; }

        public int? MateriaId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public virtual tbDocentes? Docentes { get; set; }
    }
}
