

namespace AprendeMasWeb.Models
{
	public class Grupo
	{
		public int GrupoId { get; set; }
		public string NombreGrupo { get; set; }
		public string Descripcion { get; set; }
		public string CodigoAcceso { get; set; }
		public string CodigoColor { get; set; }
		public int DocenteId { get; set; }
		public Docente Docente { get; set; }
		public ICollection<GrupoMateria> GrupoMaterias { get; set; }
	}
}
