namespace AprendeMasWeb.Recursos
{
    public static class Roles
    {
        public static string DOCENTE => "Docente";
        public static string ALUMNO => "Alumno";
        public static string ADMINISTRADOR => "Administrador";
    }
    public static class EstatusAutorizacion
    {
        public static string AUTORIZADO => "Autorizado";
        public static string DENEGADO => "Denegado";
        public static string PENDIENTE => "Pendiente";
    }

    public static class EstatusEnvioCorreoDocente
    {
        public static string NO_ENVIADO => "Sin enviar";
        public static string ENVIADO => "Enviado";
    }

    public static class RequiereDatosAdicionales
    {
        public static bool REQUERIDO => true;
        public static bool NO_REQUERIDO => false;   
    }


    public class RecursosGenerales
    {
        public static string GenerarCodigoAleatorio()
        {
            int length = 5;
            const string chars = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }

}
