using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.RestablecerPassword
{
    public class EnvioCodigoRestablecer
    {
        public required string Destinatario { get; set; }
    }
}
