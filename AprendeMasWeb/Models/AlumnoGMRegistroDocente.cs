namespace AprendeMasWeb.Models
{
    public class AlumnoGMRegistroDocente
    {
        public required List<int> AlumnosId { get; set; }

        public int MateriaId { get; set; } = 0;

        public int GrupoId { get; set; } = 0;
    }
}
