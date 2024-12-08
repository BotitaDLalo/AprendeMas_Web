using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class Archivos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int ArchivoId { get; set; }

        public required int ActividadId { get; set; }

        public required string NombreArchivo { get; set; }

        public required string DescripcionArchivo { get; set; }

        public required string UrlArchivo { get; set; }

        public required DateTime FechaSubida  { get; set; }


    }
}
