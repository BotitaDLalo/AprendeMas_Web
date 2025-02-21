namespace AprendeMasWeb.Models
{
    public class RespuestaConsultarAvisos
    {
        public int AvisoId { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string NombresDocente { get; set; }

        public string ApePaternoDocente { get; set; }

        public string ApeMaternoDocente { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int GrupoId { get; set; }

        public int MateriaId { get; set; }
    }
}
