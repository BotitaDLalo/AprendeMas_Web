﻿// Se importan los espacios de nombres necesarios para interactuar con la base de datos y la API de ASP.NET Core
using AprendeMasWeb.Data;
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
        public async Task<IActionResult> CrearMateria([FromBody] tbMaterias materia)
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

            // Buscar relaciones en la tabla GruposYMaterias
            var relacionesGrupos = _context.tbGruposMaterias.Where(gm => gm.MateriaId == id);
            // Eliminar todas las relaciones de la materia con grupos
            _context.tbGruposMaterias.RemoveRange(relacionesGrupos);

            // Buscar relaciones en la tabla AlumnosMaterias
            var relacionesAlumnos = _context.tbAlumnosMaterias.Where(am => am.MateriaId == id);
            // Eliminar todas las relaciones de la materia con alumnos
            _context.tbAlumnosMaterias.RemoveRange(relacionesAlumnos);

            await _context.SaveChangesAsync(); // Guardamos cambios para eliminar relaciones

            // Ahora eliminamos la materia
            _context.tbMaterias.Remove(materia);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Materia eliminada correctamente." });
        }

    }
}
