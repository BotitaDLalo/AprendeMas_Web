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

            // Buscar relaciones en la tabla MateriasActividades
            var relacionesMateriasActividades = _context.tbMateriasActividades.Where(ma => ma.MateriaId == id);
            // Eliminar las relaciones de la materia con las actividades
            _context.tbMateriasActividades.RemoveRange(relacionesMateriasActividades);

            // Obtener los IDs de las actividades relacionadas
            var actividadesIds = relacionesMateriasActividades.Select(ma => ma.ActividadId).ToList();

            // Eliminar las actividades relacionadas con esta materia
            var actividades = _context.tbActividades.Where(a => actividadesIds.Contains(a.ActividadId));
            _context.tbActividades.RemoveRange(actividades);

            // Eliminar las relaciones en la tabla AlumnosMaterias
            var relacionesAlumnos = _context.tbAlumnosMaterias.Where(am => am.MateriaId == id);
            _context.tbAlumnosMaterias.RemoveRange(relacionesAlumnos);

            //Busca relacion de la materia si esta dentro de un grupo
            var relacionMateriaConGrupo = _context.tbGruposMaterias.Where(mg => mg.MateriaId == id);

            // Eliminar todas las relaciones de la materia con grupo
            _context.tbGruposMaterias.RemoveRange(relacionMateriaConGrupo);


            // Ahora eliminamos la materia
            _context.tbMaterias.Remove(materia);

            // Guardar cambios para eliminar relaciones y la materia
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Materia y sus relaciones eliminadas correctamente." });
        }



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
        }
    }
}
