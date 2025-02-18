using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

namespace AprendeMasWeb.Models.DBModels
{
    public class Avisos
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

        public virtual Docentes? Docentes { get; set; }

        //public required DateTime FechaInicio { get; set; }

        //public required DateTime FechaFin {  get; set; }

        //public required bool Activo { get; set; }
    }
}
