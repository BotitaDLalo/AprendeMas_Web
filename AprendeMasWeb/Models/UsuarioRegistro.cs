using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AprendeMasWeb.Models
{
    public class UsuarioRegistro
    {
        public required string NombreUsuario { get; set; }

        public required string ApellidoPaterno { get; set; }

        public required string ApellidoMaterno { get; set; }
        public required string Nombre {  get; set; }
        public required string Correo { get; set; }
        public required string Clave { get; set; }
        public required string TipoUsuario { get; set; }
    }
}
