using System.ComponentModel.DataAnnotations;

namespace AprendeMasWeb.Models
{

    public class AlumnoGMRegistroCodigo
    {
        public required int AlumnoId { get; set; }

        public required string CodigoAcceso { get; set; }
    }

    public class AlumnoGMRegistroDocente
    {
        //public required List<int> AlumnosId { get; set; }
        public required List<string> Emails { get; set; }
        public int MateriaId { get; set; } = 0;

        public int GrupoId { get; set; } = 0;
    }


    public class AutenticacionRespuesta
    {
        public int Id { get; set; }

        public string? UserName { get; set; }

        public string? Correo { get; set; }

        public string? Rol { get; set; }

        public string? Token { get; set; }

        public string? EstaAutorizado { get; set; }

        public bool? RequiereDatosAdicionales { get; set; }
    }

    public class CancelarEnvioActividadAlumno
    {
        public required int AlumnoActividadId { get; set; }

        public required int ActividadId { get; set; }

        public required int AlumnoId { get; set; }
    }

    public class DatosFaltantesGoogle
    {
        /*Del usuario*/
        public required string Nombres { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Role { get; set; }

        /*De google*/
        public required string IdToken { get; set; }

        public string? FcmToken { get; set; }
        //public required string NombreUsuario {  get; set; }
        //public required string Email { get; set; }

    }

    public class DocentesValidacion
    {
        public int DocenteId { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Nombre { get; set; }
        public required string Email { get; set; }
        public string? Autorizado { get; set; }
        public string? EnvioCorreo { get; set; }
    }

    public class EmailConfiguration
    {
        public required string From { get; set; }

        public required string SMTPServer { get; set; }

        public int Port { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }
    }

    public class EmailVerificadoAlumno
    {
        public required string Email { get; set; }

        public string? UserName { get; set; }

        public string? Nombre { get; set; }

        public string? ApellidoPaterno { get; set; }

        public string? ApellidoMaterno { get; set; }
    }

    public class EntregableAlumno
    {
        public required int ActividadId { get; set; }

        public required int AlumnoId { get; set; }

        public string? Respuesta { get; set; }

        public required string FechaEntrega { get; set; }

        public List<string>? Enlaces { get; set; }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }


    public class GrupoMateriasRegistro
    {
        public required int DocenteId { get; set; }
        public required string NombreGrupo { get; set; }
        public string? Descripcion { get; set; }
        //public required string CodigoColor { get; set; }
        public string? CodigoAcceso { get; set; }
        public required List<MateriasP> Materias { get; set; }
    }
    public class MateriasP
    {
        public required string NombreMateria { get; set; }
        public string? Descripcion { get; set; }
        //public required string CodigoColor { get; set; }
    }

    public class Indices
    {
        public int GrupoId { get; set; } = 0;

        public int MateriaId { get; set; } = 0;
    }
    public class PeticionAlumnosEntregables
    {
        public int ActividadId { get; set; }
    }

    public class ValidarCodigoDocente
    {
        public required string Email { get; set; }
        public required string CodigoValidar { get; set; }
        public string? IdToken { get; set; }
    }
    public class MateriaConGrupo
    {
        public required string NombreMateria { get; set; }
        public string? Descripcion { get; set; }

        public required int DocenteId { get; set; }
        //public string? CodigoColor { get; set; }
        public required List<int> Grupos { get; set; }
    }
    public class PeticionConsultarAvisos
    {
        public int GrupoId { get; set; } = 0;
        public int MateriaId { get; set; } = 0;
    }

    public class PeticionCrearAviso
    {
        public int DocenteId { get; set; }

        public required string Titulo { get; set; }

        public required string Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int? GrupoId { get; set; }

        public int? MateriaId { get; set; }


    }

    public class RegistrarUsuarioGoogle
    {
        public required string IdToken { get; set; }
        //public required string Role {  get; set; }
    }

    public class RespuestaAlumnosEntregables
    {
        public int ActividadId { get; set; }
        public int Puntaje { get; set; }
        public int TotalEntregados { get; set; }
        public List<AlumnoEntregable>? AlumnosEntregables { get; set; }
    }


    public class AlumnoEntregable
    {
        public int EntregaId { get; set; }
        public int AlumnoId { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Respuesta { get; set; }
        public int Calificacion { get; set; }
    }


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

    public class TipoUsuario
    {
        [Key]
        public int TipoUsuarioId { get; set; }
        public required string Usuario { get; set; }
    }


    public class UsuarioInicioSesion
    {
        public required string Correo { get; set; }
        public required string Clave { get; set; }
    }


    public class UsuarioRegistro
    {
        public string? NombreUsuario { get; set; }

        public required string ApellidoPaterno { get; set; }

        public required string ApellidoMaterno { get; set; }
        public required string Nombre { get; set; }
        public string? Correo { get; set; }
        public required string Clave { get; set; }
        public required string TipoUsuario { get; set; }
        public string? FcmToken { get; set; }
    }

    public class VerificarGoogleIdToken
    {
        public required string IdToken { get; set; }
    }
}
