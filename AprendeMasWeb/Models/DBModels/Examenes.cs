using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Asn1.Cms;

namespace AprendeMasWeb.Models.DBModels
{
    public class Examenes
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int ExamenId { get; set; }

        public required int DocenteId { get; set; }

        public required int RubricaId { get; set; }

        public required string NombreExamen {  get; set; }
        
        public required DateTime FechaPublicacion { get; set; }

        public required DateTime FechaFinalizacion { get; set; }

        public required string Tema {  get; set; }

        public required TimeSpan Duracion { get; set; }
        public int Intentos {  get; set; } 


    }
}
