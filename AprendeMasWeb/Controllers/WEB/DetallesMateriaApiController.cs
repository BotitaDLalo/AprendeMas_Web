// Se importa el espacio de nombres para acceder a la base de datos y los controladores
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Define el controlador para la API
namespace AprendeMasWeb.Controllers.WEB
{
    // Define la ruta de la API, en este caso, 'api/[controller]' significa que el controlador será accesible a través de 'api/DetallesMateriaApi'
    [Route("api/[controller]")]
    [ApiController] // Indica que este es un controlador de API
    public class DetallesMateriaApiController : ControllerBase
    {
        // Se crea una variable privada para almacenar el contexto de la base de datos
        private readonly DataContext _context;

        // Constructor que recibe el contexto de la base de datos
        public DetallesMateriaApiController(DataContext context)
        {
            _context = context; // Se asigna el contexto a la variable _context
        }

        // Definición de un endpoint HTTP GET para obtener los detalles de una materia específica
        [HttpGet("ObtenerDetallesMateria/{materiaId}/{docenteId}")]
        public async Task<IActionResult> ObtenerDetallesMateria(int materiaId, int docenteId)
        {
            // Se consulta la base de datos usando Entity Framework para obtener los detalles de la materia
            var materiaDetalles = await _context.tbMaterias
                // Filtro que busca la materia por su id y el id del docente
                .Where(m => m.MateriaId == materiaId && m.DocenteId == docenteId)
                // Selección de los campos que se devolverán: NombreMateria, CodigoAcceso, CodigoColor, DocenteId
                .Select(m => new
                {
                    NombreMateria = m.NombreMateria, // Nombre de la materia
                    CodigoAcceso = m.CodigoAcceso, // Código de acceso de la materia
                    CodigoColor = m.CodigoColor, // Color asociado a la materia
                    DocenteId = m.DocenteId // ID del docente
                })
                // Obtiene el primer resultado que coincide con la búsqueda o null si no se encuentra ninguno
                .FirstOrDefaultAsync();

            // Verifica si no se encontraron detalles de la materia
            if (materiaDetalles == null)
            {
                // Devuelve un error 404 con un mensaje indicando que no se encontró la materia
                return NotFound(new { mensaje = "Materia No Encontrada O Sin Permiso" });
            }

            // Si se encuentran los detalles, devuelve un resultado exitoso con los detalles de la materia
            return Ok(materiaDetalles);
        }

        [HttpGet("BuscarAlumnosPorCorreo")]
        public async Task<IActionResult> BuscarAlumnosPorCorreo(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("El criterio de búsqueda no puede estar vacío.");
            }

            // Buscar usuarios que coincidan con el correo ingresado o cualquier parte de nombre o apellido
            var usuarios = await _context.Users
                .Where(u => u.Email.Contains(query)) // Buscar por correo
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            // Buscar alumnos registrados que coincidan con el correo o nombre completo (nombre, apellidos)
            var alumnosConCorreo = await _context.tbAlumnos
                .Where(a => a.Nombre.Contains(query) || // Buscar por nombre
                            a.ApellidoPaterno.Contains(query) || // Buscar por apellido paterno
                            a.ApellidoMaterno.Contains(query) || // Buscar por apellido materno
                            usuarios.Select(u => u.Id).Contains(a.UserId)) // Buscar por correo
                .Select(a => new
                {
                    a.IdentityUser.Email,
                    a.Nombre,
                    a.ApellidoPaterno,
                    a.ApellidoMaterno
                })
                .ToListAsync();

            return Ok(alumnosConCorreo);
        }

        //controlador para unir materia con alumno

        // Método para buscar el alumno por correo y asignarlo a la materia si no está asignado
        [HttpPost("AsignarAlumnoMateria")]
        public async Task<IActionResult> AsignarAlumnoMateria([FromQuery] string correo, [FromQuery] int materiaId)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return BadRequest("El correo no puede estar vacío.");
            }

            // Buscar el alumno por correo
            var alumno = await _context.tbAlumnos
                .Where(a => a.IdentityUser.Email == correo)
                .Select(a => new
                {
                    a.AlumnoId
                })
                .FirstOrDefaultAsync();

            if (alumno == null)
            {
                return NotFound(new { mensaje = "Alumno no encontrado con el correo proporcionado." });
            }

            // Verificar si ya existe la relación en la tabla alumnosMaterias
            var relacionExistente = await _context.tbAlumnosMaterias
                .Where(am => am.AlumnoId == alumno.AlumnoId && am.MateriaId == materiaId)
                .FirstOrDefaultAsync();

            if (relacionExistente != null)
            {
                return BadRequest(new { mensaje = "El alumno ya está asignado a esta materia." });
            }

            // Si no existe la relación, agregarla a la tabla alumnosMaterias
            var nuevaRelacion = new AlumnosMaterias
            {
                AlumnoId = alumno.AlumnoId,
                MateriaId = materiaId
            };

            await _context.tbAlumnosMaterias.AddAsync(nuevaRelacion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Alumno asignado a la materia exitosamente." });
        }

    }
}
