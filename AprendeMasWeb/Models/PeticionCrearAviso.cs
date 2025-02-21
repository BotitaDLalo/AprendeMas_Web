namespace AprendeMasWeb.Models
{
    public class PeticionCrearAviso
    {
        public int DocenteId { get; set; }

        public required string Titulo { get; set; }

        public required string Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; }
        
        public int? GrupoId {  get; set; }

        public int? MateriaId { get; set; }


    }
}
