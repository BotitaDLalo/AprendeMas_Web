// Se importa el espacio de nombres para acceder a la base de datos y los controladores
using AprendeMasWeb.Data;
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
        public async Task<IActionResult> BuscarAlumnosPorCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return BadRequest("El correo no puede estar vacío.");
            }

            // Buscar los usuarios en aspnetusers que coincidan con el correo ingresado
            var usuarios = await _context.Users
                .Where(u => u.Email.Contains(correo))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            // Buscar alumnos registrados que coincidan con los usuarios encontrados
            var alumnosConCorreo = await _context.tbAlumnos
                .Where(a => usuarios.Select(u => u.Id).Contains(a.UserId))
                .Select(a => new
                {
                    Email = a.IdentityUser.Email // Asegurar que el campo se llame "Email"
                })
                .ToListAsync();

            return Ok(alumnosConCorreo);
        }

        /*
        [HttpGet("BuscarAlumnosPorCorreo")]
        public async Task<IActionResult> BuscarAlumnosPorCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return BadRequest("El correo no puede estar vacío.");
            }

            // Buscar los usuarios en AspNetUsers que coincidan con el correo ingresado
            var usuarios = await _context.Users
                .Where(u => u.Email.Contains(correo))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            // Buscar alumnos registrados que coincidan con los usuarios encontrados
            var alumnosConCorreo = await _context.tbAlumnos
                .Where(a => usuarios.Select(u => u.Id).Contains(a.UserId))
                .Select(a => new
                {
                    a.Nombre,
                    a.ApellidoPaterno,
                    a.ApellidoMaterno,
                    a.IdentityUser.Email
                })
                .ToListAsync();

            return Ok(alumnosConCorreo);
        }*/

    }
}
