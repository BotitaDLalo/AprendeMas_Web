using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
    public class AlumnosTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FCMToken { get; set; }

        public required int AlumnoId { get; set; }

        public required string Token { get; set; }

        public virtual Alumnos? Alumnos { get; set; }
    }
}
