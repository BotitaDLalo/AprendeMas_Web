using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.DBModels
{
	public class EventosAgendaAlumno
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int EventoAlumnoId { get; set; }

		public required int AlumnoId { get; set; }

		[ForeignKey("AlumnoId")]
		public virtual tbAlumnos? Alumno { get; set; }

		public required DateTime FechaInicio { get; set; }

		public required DateTime FechaFinal { get; set; }

		public required string Titulo { get; set; }

		public required string Descripcion { get; set; }

		public required string Color { get; set; }
	}
}

