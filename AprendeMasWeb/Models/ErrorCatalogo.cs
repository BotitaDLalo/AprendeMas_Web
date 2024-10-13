using static Google.Apis.Requests.RequestError;

namespace AprendeMasWeb.Models
{
    public static class ErrorCatalogo
    {
        public enum ErrorCodigos
        {
            CredencialesInvalidas = 1001,
            UsuarioNoEncontrado = 1002,
            NombreUsuarioExistente = 1003,
            CorreoUsuarioExistente = 1004,
            tokenInvalido = 1005,
        }

        private static readonly Dictionary<ErrorCodigos, string> DiccionarioErrores = new()
        {
            {ErrorCodigos.CredencialesInvalidas,"El correo electrónico ya existe" },
            {ErrorCodigos.UsuarioNoEncontrado,"Usuario inexistente" },
            {ErrorCodigos.NombreUsuarioExistente,"El nombre de usuario ya esta en uso" },
            {ErrorCodigos.tokenInvalido,"Token de autenticacion invalido" },
        };


        public static string GetMensajeError(ErrorCodigos errorCodigos)
        {
            return DiccionarioErrores.TryGetValue(errorCodigos, out var message) ? message : "Error desconocido";
        }


}
}
