// Se importan los espacios de nombres necesarios para interactuar con la base de datos y la API de ASP.NET Core
using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    // Se define la ruta base para este controlador API
    [Route("api/[controller]")]
    // Indica que este controlador es para una API
    [ApiController]
    public class MateriasApiController : ControllerBase
    {
        // Se declara el contexto de la base de datos para interactuar con los datos de la aplicación
        private readonly DataContext _context;

        // Constructor que recibe el contexto de datos para poder interactuar con la base de datos
        public MateriasApiController(DataContext context)
        {
            _context = context; // Asigna el contexto de datos a la variable de la clase
        }

        // Controlador para crear una nueva materia mediante una solicitud POST (API)
        [HttpPost("CrearMateria")]
        public async Task<IActionResult> CrearMateria([FromBody] Materias materia)
        {
            // Verifica si el modelo enviado es válido (ejemplo: los datos de la materia están completos)
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, devuelve un mensaje de error con un estado BadRequest
                return BadRequest("Datos de materia invalido.");
            }

            // Genera un código de acceso para la materia
            materia.CodigoAcceso = ObtenerClaveMateria();
            // Agrega la materia a la base de datos
            _context.tbMaterias.Add(materia);
            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();
            // Retorna una respuesta exitosa con un mensaje y el ID de la materia creada
            return Ok(new { mensaje = "Materia creada con exito.", materiaId = materia.MateriaId });
        }

        // Método privado que genera una clave aleatoria de 10 caracteres para la materia
        private string ObtenerClaveMateria()
        {
            var random = new Random(); // Crea una instancia de la clase Random
            // Genera una cadena de 10 caracteres aleatorios entre A y Z
            return new string(Enumerable.Range(0, 10).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }

        // Controlador para obtener las materias que no tienen asignado un grupo
        [HttpGet("ObtenerMateriasSinGrupo/{docenteId}")]
        public async Task<IActionResult> ObtenerMateriasSinGrupo(int docenteId)
        {
            // Consulta las materias que pertenecen al docente con el ID proporcionado
            // Obtiene las matgerias del docente que No estan en la tabla GruposYMaterias
            var materiasSinGrupo = await _context.tbMaterias
                .Where(m => m.DocenteId == docenteId &&
                    !_context.tbGruposMaterias.Any(gm => gm.MateriaId == m.MateriaId))
                .ToListAsync();
            return Ok(materiasSinGrupo);
        }

        // Controlador para eliminar una materia por su ID
        [HttpDelete("EliminarMateria/{id}")]
        public async Task<IActionResult> EliminarMateria(int id)
        {
            // Buscar la materia en la base de datos
            var materia = await _context.tbMaterias.FindAsync(id);
            if (materia == null)
            {
                return NotFound(new { mensaje = "La materia no existe" });
            }

            // Obtener y eliminar las actividades directamente asociadas a esta materia
            var actividades = _context.tbActividades.Where(a => a.MateriaId == id);
            _context.tbActividades.RemoveRange(actividades);

            //Buscar y eliminar los avisos directamente asociadas a esta materia
            var avisos = _context.tbAvisos.Where(a => a.MateriaId == id);
            _context.tbAvisos.RemoveRange(avisos);


            // Eliminar las relaciones en la tabla AlumnosMaterias
            var relacionesAlumnos = _context.tbAlumnosMaterias.Where(am => am.MateriaId == id);
            _context.tbAlumnosMaterias.RemoveRange(relacionesAlumnos);

            // Buscar y eliminar las relaciones de la materia con los grupos
            var relacionMateriaConGrupo = _context.tbGruposMaterias.Where(mg => mg.MateriaId == id);
            _context.tbGruposMaterias.RemoveRange(relacionMateriaConGrupo);


            // Ahora eliminamos la materia
            _context.tbMaterias.Remove(materia);

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Materia y sus relaciones eliminadas correctamente." });
        }

        /*
        //Controlador actualiza materia, aun no funciona
        [HttpPut("ActualizarMateria/{id}")]
        public async Task<IActionResult> ActualizarMateria(int id, [FromBody] Materias materiaActualizada)
        {
            // Buscar la materia en la base de datos
            var materia = await _context.tbMaterias.FindAsync(id);

            // Si la materia no existe, devolver un error
            if (materia == null)
            {
                return NotFound(new { mensaje = "La materia no existe." });
            }

            // Actualizar los campos nombreMateria y descripcion
            materia.NombreMateria = materiaActualizada.NombreMateria;
            materia.Descripcion = materiaActualizada.Descripcion;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Materia actualizada correctamente." });
        }*/
        [HttpPut("ActualizarMateria/{materiaId}")]
        public async Task<IActionResult> ActualizarMateria(int materiaId, [FromBody] Materias materiaDto)
        {
            var materiaExistente = await _context.tbMaterias.FindAsync(materiaId);

            if (materiaExistente == null)
            {
                return NotFound("Materia no encontrada.");
            }

            // Solo se actualizan los campos que llegaron
            if (!string.IsNullOrEmpty(materiaDto.NombreMateria))
            {
                materiaExistente.NombreMateria = materiaDto.NombreMateria;
            }

            if (!string.IsNullOrEmpty(materiaDto.Descripcion))
            {
                materiaExistente.Descripcion = materiaDto.Descripcion;
            }
            await _context.SaveChangesAsync();

            return Ok(materiaExistente);
        }

    }
}
