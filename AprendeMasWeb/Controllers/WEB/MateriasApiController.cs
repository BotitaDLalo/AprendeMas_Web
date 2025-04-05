// Se importan los espacios de nombres necesarios para interactuar con la base de datos y la API de ASP.NET Core
using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Authorize(Roles = "Docente")]
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


        // Controlador para crear una nueva materia y asociarla a un grupo ya existente.
        [HttpPost("AgregarMateriaAlGrupo/{grupoId}")]
        public async Task<IActionResult> AgregarMateriaAlGrupo(int grupoId, [FromBody] tbMaterias materia)
        {
            // Verifica si el modelo enviado es válido
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos de materia inválidos.");
            }

            // Verifica si el grupo existe
            var grupoExiste = await _context.tbGrupos.AnyAsync(g => g.GrupoId == grupoId);
            if (!grupoExiste)
            {
                return NotFound("El grupo especificado no existe.");
            }

            // Genera un código de acceso para la materia
            materia.CodigoAcceso = ObtenerClaveMateria();
            // Agrega la materia a la base de datos
            _context.tbMaterias.Add(materia);
            await _context.SaveChangesAsync(); // Guarda para obtener el ID de la materia generada

            // Crea la relación en la tabla tbGruposMaterias
            var grupoMateria = new tbGruposMaterias
            {
                GrupoId = grupoId,
                MateriaId = materia.MateriaId
            };

            _context.tbGruposMaterias.Add(grupoMateria);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Materia creada y asociada al grupo con éxito.", materiaId = materia.MateriaId });
        }

        // Método privado que genera una clave aleatoria de 10 caracteres para la materia
        private string ObtenerClaveMateria()
        {
            var random = new Random(); // Crea una instancia de la clase Random
            // Genera una cadena de 10 caracteres aleatorios entre A y Z
            return new string(Enumerable.Range(0, 10).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }
        



        // Controlador para obtener las materias sin grupo y sus 2 actividades más recientes
        [HttpGet("ObtenerMateriasSinGrupo/{docenteId}")]
        public async Task<IActionResult> ObtenerMateriasSinGrupo(int docenteId)
        {
            var materiasSinGrupo = await _context.tbMaterias
                .Where(m => m.DocenteId == docenteId &&
                    !_context.tbGruposMaterias.Any(gm => gm.MateriaId == m.MateriaId))
                .ToListAsync(); // Obtener materias primero

            // Para cada materia, buscar sus 2 actividades más recientes
            var resultado = new List<object>();

            foreach (var materia in materiasSinGrupo)
            {
                var actividadesRecientes = await _context.tbActividades
                    .Where(a => a.MateriaId == materia.MateriaId)
                    .OrderByDescending(a => a.FechaCreacion)
                    .Take(2)
                    .Select(a => new
                    {
                        a.ActividadId,
                        a.NombreActividad,
                        a.FechaCreacion
                    })
                    .ToListAsync(); // Obtener actividades recientes de esta materia

                resultado.Add(new
                {
                    materia.MateriaId,
                    materia.NombreMateria,
                    materia.Descripcion,
                    materia.CodigoColor,
                    materia.CodigoAcceso,
                    materia.DocenteId,
                    ActividadesRecientes = actividadesRecientes // Se asignan correctamente aquí
                });
            }

            return Ok(resultado);
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

            // Obtener las actividades asociadas a esta materia
            var actividades = _context.tbActividades.Where(a => a.MateriaId == id).ToList();
            var actividadIds = actividades.Select(a => a.ActividadId).ToList();

            // Obtener los registros en la tabla alumnosActividades relacionados con estas actividades
            var alumnosActividades = _context.tbAlumnosActividades.Where(aa => actividadIds.Contains(aa.ActividadId)).ToList();
            var alumnosActividadIds = alumnosActividades.Select(aa => aa.AlumnoActividadId).ToList();

            // Obtener los entregables relacionados con alumnosActividades
            var entregables = _context.tbEntregablesAlumno.Where(ea => alumnosActividadIds.Contains(ea.AlumnoActividadId)).ToList();
            var entregaIds = entregables.Select(ea => ea.EntregaId).ToList();

            // Obtener las calificaciones asociadas a los entregables
            var calificaciones = _context.tbCalificaciones.Where(c => entregaIds.Contains(c.EntregaId)).ToList();

            // Eliminar en orden inverso para evitar conflictos de claves foráneas
            _context.tbCalificaciones.RemoveRange(calificaciones);
            _context.tbEntregablesAlumno.RemoveRange(entregables);
            _context.tbAlumnosActividades.RemoveRange(alumnosActividades);
            _context.tbActividades.RemoveRange(actividades);

            // Obtener y eliminar los avisos directamente asociados a esta materia
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


        // Método para obtener los detalles de una materia específica por ID
        [HttpGet("ObtenerMateria/{id}")]
        public async Task<IActionResult> ObtenerMateria(int id)
        {
            var materia = await _context.tbMaterias.FindAsync(id);

            if (materia == null)
            {
                return NotFound("La materia no existe.");
            }

            return Ok(materia);  // Devuelve la materia encontrada
        }


        //Actualizar Materia
        [HttpPut("ActualizarMateria")]
        public async Task<IActionResult> ActualizarMateria([FromBody] tbMaterias model)
        {
            try
            {
                var materia = await _context.tbMaterias.FindAsync(model.MateriaId);
                if (materia == null)
                    return NotFound(new { mensaje = "Materia no encontrada" });

                materia.NombreMateria = model.NombreMateria;
                materia.Descripcion = model.Descripcion;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Materia actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la materia", error = ex.Message });
            }
        }

    }
}
