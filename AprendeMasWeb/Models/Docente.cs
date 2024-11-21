
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace AprendeMasWeb.Models
{
	public class Docente
	{
		public int DocenteId { get; set; }
		public ICollection<Grupo> Grupos { get; set; }
		public ICollection<Materia> Materias { get; set; }
		public ICollection<Actividad> Actividades { get; set; }
		public ICollection<Aviso> Avisos { get; set; }
		public ICollection<EventoAgenda> EventosAgenda { get; set; }
	}
}
