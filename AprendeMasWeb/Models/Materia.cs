

namespace AprendeMasWeb.Models
{
	public class Materia
	{
		public int MateriaId { get; set; }
		public string NombreMateria { get; set; }
		public string Descripcion { get; set; }
		public int DocenteId { get; set; }
		public Docente Docente { get; set; }
		public ICollection<GrupoMateria> GrupoMaterias { get; set; }
	}
}
