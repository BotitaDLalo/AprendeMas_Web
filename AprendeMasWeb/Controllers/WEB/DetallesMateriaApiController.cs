// Se importa el espacio de nombres para acceder a la base de datos y los controladores
using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Google;
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

        // Método para obtener la lista de alumnos que están dentro de la materia
        [HttpGet("ObtenerAlumnosPorMateria/{materiaId}")]
        public async Task<IActionResult> ObtenerAlumnosPorMateria(int materiaId)
        {
            var alumnos = await _context.tbAlumnosMaterias
                .Where(am => am.MateriaId == materiaId)
                .Join(_context.tbAlumnos,
                    am => am.AlumnoId,
                    a => a.AlumnoId,
                    (am, a) => new
                    {
                        am.AlumnoMateriaId, // Se agrega el AlumnoMateriaId
                        a.AlumnoId,
                        a.Nombre,
                        a.ApellidoPaterno,
                        a.ApellidoMaterno
                    })
                .OrderBy(a => a.ApellidoPaterno) // Ordena por apellido paterno
                .ThenBy(a => a.ApellidoMaterno)  // Si hay empate en el apellido paterno, ordena por el apellido materno
                .ThenBy(a => a.Nombre)           // Si hay empate en el apellido materno, ordena por el nombre
                .ToListAsync();
            return Ok(alumnos);
        }

        //Eliminar a un alumno de la materia.
        [HttpDelete("EliminarAlumnoDeMateria/{idEnlace}")]
        public async Task<IActionResult> EliminarAlumnoDeMateria(int idEnlace)
        {
            try
            {
                //Buscar el registro en la base de datos
                var alumnoMateria = await _context.tbAlumnosMaterias
                    .FirstOrDefaultAsync(am => am.AlumnoMateriaId == idEnlace);

                //Si no se encuentra se retorna un error
                if(alumnoMateria == null)
                {
                    return NotFound(new { mensaje = "No se encontro el alumno en la materia" });
                }

                //Eliminar el registro de la base de datos
                _context.tbAlumnosMaterias.Remove(alumnoMateria);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Alumno eliminado de la materia correctamente." });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar al alumno.", error = ex.Message });
            }
        }

        //Controlador api que crea actividades
        //Guarda la actividad directo en tabla actividades. 
        [HttpPost("CrearActividad")]
        public async Task<IActionResult> CrearActividad([FromBody] Actividades actividadDto)
        {
            if (actividadDto == null)
            {
                return BadRequest(new { mensaje = "Datos inválidos." });
            }

            
            try
            {
                 // Crear nueva actividad
                 var nuevaActividad = new Actividades
                 {
                     NombreActividad = actividadDto.NombreActividad,
                     Descripcion = actividadDto.Descripcion,
                     FechaCreacion = DateTime.Now,
                     FechaLimite = actividadDto.FechaLimite,
                     TipoActividadId = actividadDto.TipoActividadId,
                     Puntaje = actividadDto.Puntaje,
                     MateriaId = actividadDto.MateriaId
                 };

                 _context.tbActividades.Add(nuevaActividad);
                 await _context.SaveChangesAsync(); // Guarda la actividad para obtener su ID
                 return Ok(new { mensaje = "Actividad creada con éxito", actividadId = nuevaActividad.ActividadId });
            }  
            catch (Exception ex)
            {
                    return StatusCode(500, new { mensaje = "Error al crear la actividad", error = ex.Message });
            }
        }

        //Controlador que obtiene  todo lo de actividades que pertecenen a esa materia
        [HttpGet("ObtenerActividadesPorMateria/{materiaId}")]
        public async Task<IActionResult> ObtenerActividadesPorMateria(int materiaId)
        {
            try
            {
                var actividades = await _context.tbActividades
                .Where(a => a.MateriaId == materiaId)
                .Select(a => new
                 {
                     a.ActividadId,  
                     a.NombreActividad,
                     a.Descripcion,
                     a.FechaCreacion,
                     a.FechaLimite,
                     a.Puntaje
                })
                .ToListAsync();
                if (actividades == null || actividades.Count == 0)
                {
                    return NotFound(new { mensaje = "No hay actividades registradas para esta materia." });
                }

                return Ok(actividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las actividades", error = ex.Message });
            }
        }


            [HttpDelete("EliminarActividad/{id}")]
            public async Task<IActionResult> EliminarActividad(int id)
            {
                // Buscar el registro en la tabla materiasActividades con el ID recibido
                var Actividad = await _context.tbActividades
                    .FirstOrDefaultAsync(a => a.ActividadId == id);

                if (Actividad == null)
                {
                    return NotFound("No se encontró el registro en Actividades.");
                }

                // Eliminar el registro de materiasActividades primero
                _context.tbActividades.Remove(Actividad);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Actividad eliminada correctamente." });
            }

        //Controlador para crear un aviso
        [HttpPost("CrearAviso")]
        public async Task<IActionResult> CrearAviso([FromBody] Avisos avisos)
        {
            if(avisos == null)
            {
                return BadRequest(new { mensaje = "Datos Invalidos." });
            }
            try
            {
                var nuevoAviso = new Avisos
                {
                    DocenteId = avisos.DocenteId,
                    Titulo = avisos.Titulo,
                    Descripcion = avisos.Descripcion,
                    GrupoId = avisos.GrupoId,
                    MateriaId = avisos.MateriaId,
                    FechaCreacion = DateTime.Now
                };
                _context.tbAvisos.Add(nuevoAviso);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Aviso creado con éxito"});

            }catch(Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el aviso", error = ex.Message });
            }
        }

        //Controlador para obtener avisos
        [HttpGet("ObtenerAvisos")]
        public async Task<IActionResult> ObtenerAvisos([FromQuery] int IdMateria)
        {
            try
            {
                var avisos = await _context.tbAvisos
                    .Where(a => a.MateriaId == IdMateria)
                    .Select(a => new
                    {
                        a.AvisoId,
                        a.Titulo,
                        a.Descripcion,
                        a.FechaCreacion
                    })
                    .ToListAsync();

                return Ok(avisos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los avisos", error = ex.Message });
            }
        }

    }
}
