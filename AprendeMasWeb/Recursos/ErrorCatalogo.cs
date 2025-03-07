using static Google.Apis.Requests.RequestError;

namespace AprendeMasWeb.Recursos
{
    public static class ErrorCatalogo
    {

        /*
         1000 - Errores autentificacion
         */
        public enum ErrorCodigos
        {
            CredencialesInvalidas = 1001,
            CorreoUsuarioExistente = 1002,
            tokenInvalido = 1003,
            codigoAutorizacionInvalido = 1004,
            codigoAutorizacionExpirado = 1005,
            nombreUsuarioUsado = 1006
        }
        private static readonly Dictionary<ErrorCodigos, string> DiccionarioErrores = new()
        {
            {ErrorCodigos.CredencialesInvalidas,"Correo o contraseña son incorrectos." },
            {ErrorCodigos.CorreoUsuarioExistente,"Este correo ya está asociado a otra cuenta." },
            {ErrorCodigos.tokenInvalido,"Token de autenticacion invalido." },
            {ErrorCodigos.nombreUsuarioUsado,"Nombre de usuario ya esta en uso." }
        };


        public static string GetMensajeError(ErrorCodigos errorCodigos)
        {
            return DiccionarioErrores.TryGetValue(errorCodigos, out var message) ? message : "Error desconocido";
        }


    }
}
