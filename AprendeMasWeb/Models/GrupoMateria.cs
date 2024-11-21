using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{
	public class GrupoMateria
	{
		[Key] // Clave primaria
		public int GrupoMateriasId { get; set; }
		public int GrupoId { get; set; }
		public Grupo Grupo { get; set; }
		public int MateriaId { get; set; }
		public Materia Materia { get; set; }
	}
}
