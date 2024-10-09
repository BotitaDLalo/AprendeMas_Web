using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models.RestablecerPassword
{
    public class ValidacionPasswords
    {
        [Required(ErrorMessage = "Es necesario confirmar la contraseña")]
        [DataType(DataType.Password)]
        public required string NuevaPassword { get; set; }

        [Required(ErrorMessage = "Es necesario confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmarPassword { get; set; }
    }
}
