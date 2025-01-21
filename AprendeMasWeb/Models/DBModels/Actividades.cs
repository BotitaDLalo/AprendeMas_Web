using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AprendeMasWeb.Models.DBModels;

namespace AprendeMasWeb.Models
{
    public class Actividades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActividadId { get; set; }

        public required string NombreActividad { get; set; }

        public required string Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaLimite { get; set; }

        public int TipoActividadId { get; set; }

        public int Puntaje {  get; set; }

        public int MateriaId { get; set; }


        public virtual ICollection<AlumnosActividades>? AlumnosActividades { get; set; }    
        public virtual ICollection<MateriasActividades>? MateriasActividades { get; set; }
        public virtual TiposActividades? TiposActividades { get; set; }
        public virtual ICollection<Calificaciones>? Calificaciones { get; set; } 
    }
}
