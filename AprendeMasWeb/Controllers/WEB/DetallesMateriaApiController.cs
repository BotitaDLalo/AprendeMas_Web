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
        public async Task<IActionResult> BuscarAlumnosPorCorreo(string query, int materiaId)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("El criterio de búsqueda no puede estar vacío.");
            }

            // Buscar usuarios que coincidan con el correo ingresado
            var usuarios = await _context.Users
                .Where(u => u.Email.Contains(query))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            // Buscar alumnos registrados que coincidan con el criterio
            var alumnosConCorreo = await _context.tbAlumnos
                .Where(a => (a.Nombre.Contains(query) ||
                             a.ApellidoPaterno.Contains(query) ||
                             a.ApellidoMaterno.Contains(query) ||
                             usuarios.Select(u => u.Id).Contains(a.UserId)) &&
                             !_context.tbAlumnosMaterias
                                .Any(am => am.AlumnoId == a.AlumnoId && am.MateriaId == materiaId)) // Excluir alumnos ya asignados
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
            var nuevaRelacion = new tbAlumnosMaterias
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
                if (alumnoMateria == null)
                {
                    return NotFound(new { mensaje = "No se encontro el alumno en la materia" });
                }

                //Eliminar el registro de la base de datos
                _context.tbAlumnosMaterias.Remove(alumnoMateria);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Alumno eliminado de la materia correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar al alumno.", error = ex.Message });
            }
        }

        // Controlador api que crea actividades y asigna a los alumnos
        [HttpPost("CrearActividad")]
        public async Task<IActionResult> CrearActividad([FromBody] tbActividades actividadDto)
        {
            if (actividadDto == null)
            {
                return BadRequest(new { mensaje = "Datos inválidos." });
            }

            // Validar que la fecha límite sea en el futuro
            if (actividadDto.FechaLimite <= DateTime.Now)
            {
                return BadRequest(new { mensaje = "La fecha límite debe ser en el futuro." });
            }

            // Verificar que la materia exista en la base de datos
            var materiaExiste = await _context.tbMaterias.AnyAsync(m => m.MateriaId == actividadDto.MateriaId);
            if (!materiaExiste)
            {
                return BadRequest(new { mensaje = "La materia especificada no existe." });
            }

            // Verificar que el tipo de actividad exista en la base de datos
            var tipoActividadExiste = await _context.cTiposActividades.AnyAsync(t => t.TipoActividadId == actividadDto.TipoActividadId);
            if (!tipoActividadExiste)
            {
                return BadRequest(new { mensaje = "El tipo de actividad especificado no existe." });
            }

            try
            {
                // Crear la nueva actividad
                var nuevaActividad = new tbActividades
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
                await _context.SaveChangesAsync(); // Guarda la actividad y genera el ID

                // Obtener los alumnos que pertenecen a la materia
                var alumnosMateria = await _context.tbAlumnosMaterias
                    .Where(am => am.MateriaId == actividadDto.MateriaId)
                    .Select(am => am.AlumnoId)
                    .ToListAsync();

                // Crear registros en la tabla AlumnoActividad para cada alumno
                foreach (var alumnoId in alumnosMateria)
                {
                    var alumnoActividad = new tbAlumnosActividades
                    {
                        ActividadId = nuevaActividad.ActividadId,
                        AlumnoId = alumnoId,
                        FechaEntrega = DateTime.Now, // Asignar la fecha de creación como la fecha de entrega Despues se actualiza cuando el alumno lo entrega
                        EstatusEntrega = false // Inicialmente no entregado
                    };

                    _context.tbAlumnosActividades.Add(alumnoActividad);
                }

                // Guardar los cambios en la tabla AlumnoActividad
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Actividad creada y asignada a los alumnos con éxito", actividadId = nuevaActividad.ActividadId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la actividad", error = ex.Message });
            }
        }


        // Método para obtener los detalles de una actividad específica por ID
        [HttpGet("ObtenerActividad/{id}")]
        public async Task<IActionResult> ObtenerActividad(int id)
        {
            var actividad = await _context.tbActividades.FindAsync(id);

            if (actividad == null)
            {
                return NotFound("La actividad no existe.");
            }

            return Ok(actividad);  // Devuelve la materia encontrada
        }

        [HttpPut("ActualizarActividad")]
        public async Task<IActionResult> ActualizarActividad([FromBody] tbActividades model)
        {
            if (model == null)
                return BadRequest(new { mensaje = "Datos inválidos" });

            try
            {
                var actividad = await _context.tbActividades.FindAsync(model.ActividadId);
                if (actividad == null)
                    return NotFound(new { mensaje = "Actividad no encontrada" });

                // Evita sobrescribir con valores nulos
                actividad.NombreActividad = model.NombreActividad ?? actividad.NombreActividad;
                actividad.Descripcion = model.Descripcion ?? actividad.Descripcion;
                actividad.FechaLimite = model.FechaLimite != default ? model.FechaLimite : actividad.FechaLimite;
                actividad.Puntaje = model.Puntaje != 0 ? model.Puntaje : actividad.Puntaje;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Actividad actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la actividad", error = ex.Message });
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
            // Buscar la actividad en la tabla tbActividades
            var actividad = await _context.tbActividades
                .FirstOrDefaultAsync(a => a.ActividadId == id);

            if (actividad == null)
            {
                return NotFound("No se encontró el registro en Actividades.");
            }

            // Buscar registros en la tabla alumnosActividades relacionados con la actividad
            var alumnosActividades = await _context.tbAlumnosActividades
                .Where(aa => aa.ActividadId == id)
                .ToListAsync();

            foreach (var alumnoActividad in alumnosActividades)
            {
                int alumnoActividadId = alumnoActividad.AlumnoActividadId;

                // Buscar registros en la tabla entregablesAlumno relacionados con alumnoActividadId
                var entregables = await _context.tbEntregablesAlumno
                    .Where(e => e.AlumnoActividadId == alumnoActividadId)
                    .ToListAsync();

                foreach (var entrega in entregables)
                {
                    int entregaId = entrega.EntregaId;

                    // Buscar registros en la tabla calificaciones relacionados con entregaId
                    var calificaciones = await _context.tbCalificaciones
                        .Where(c => c.EntregaId == entregaId)
                        .ToListAsync();

                    // Eliminar calificaciones
                    _context.tbCalificaciones.RemoveRange(calificaciones);
                }

                // Eliminar entregables
                _context.tbEntregablesAlumno.RemoveRange(entregables);
            }

            // Eliminar alumnosActividades
            _context.tbAlumnosActividades.RemoveRange(alumnosActividades);

            // Finalmente, eliminar la actividad
            _context.tbActividades.Remove(actividad);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "Actividad y registros relacionados eliminados correctamente." });
        }


        //Controlador para crear un aviso funciona desde dentro de la materia
        [HttpPost("CrearAviso")]
        public async Task<IActionResult> CrearAviso([FromBody] tbAvisos avisos)
        {
            if (avisos == null)
            {
                return BadRequest(new { mensaje = "Datos Invalidos." });
            }
            try
            {
                var nuevoAviso = new tbAvisos
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
                return Ok(new { mensaje = "Aviso creado con éxito" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el aviso", error = ex.Message });
            }
        }

        //Crea aviso cuando pues se crea un aviso desde el grupo
        [HttpPost("CrearAvisoPorGrupo")]
        public async Task<IActionResult> CrearAvisoPorGrupo([FromBody] tbAvisos datos)
        {
            if (datos == null || datos.GrupoId == null || string.IsNullOrWhiteSpace(datos.Titulo) || string.IsNullOrWhiteSpace(datos.Descripcion))
            {
                return BadRequest(new { mensaje = "Datos inválidos." });
            }

            try
            {
                int? grupoId = datos.GrupoId;
                string titulo = datos.Titulo;
                string descripcion = datos.Descripcion;

                // Buscar todas las materias asociadas a ese GrupoId en la tabla GruposYMaterias
                var materiasRelacionadas = await _context.tbGruposMaterias
                    .Where(gm => gm.GrupoId == grupoId)
                    .Select(gm => gm.MateriaId)
                    .ToListAsync();

                if (!materiasRelacionadas.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron materias asociadas a este grupo." });
                }

                // Crear un aviso para cada materia relacionada con el grupo
                var avisos = materiasRelacionadas.Select(materiaId => new tbAvisos
                {
                    DocenteId = datos.DocenteId, // Asegurar que venga en los datos
                    Titulo = titulo,
                    Descripcion = descripcion,
                    GrupoId = grupoId,
                    MateriaId = materiaId,
                    FechaCreacion = DateTime.Now
                }).ToList();

                _context.tbAvisos.AddRange(avisos);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Avisos creados con éxito", cantidad = avisos.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear los avisos", error = ex.Message });
            }
        }




        //Controlador para eliminar un aviso
        [HttpDelete("EliminarAviso/{id}")]
        public async Task<IActionResult> EliminarAviso(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { mensaje = "ID de aviso inválido." });
            }
            try
            {
                // Buscar el aviso por su ID
                var aviso = await _context.tbAvisos.FindAsync(id);

                // Si no se encuentra el aviso
                if (aviso == null)
                {
                    return NotFound(new { mensaje = "Aviso no encontrado." });
                }

                // Eliminar el aviso
                _context.tbAvisos.Remove(aviso);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Aviso eliminado con éxito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el aviso", error = ex.Message });
            }
        }


        //Controlador para obtener avisos para la vista
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

        //Controlador para obtener informacion de un aviso para despeus editar
        [HttpGet("ObtenerAvisoPorId")]
        public async Task<IActionResult> ObtenerAvisoPorId([FromQuery] int avisoId)
        {
            try
            {
                var aviso = await _context.tbAvisos
                    .Where(a => a.AvisoId == avisoId)
                    .Select(a => new
                    {
                        a.AvisoId,
                        a.Titulo,
                        a.Descripcion
                    })
                    .FirstOrDefaultAsync();

                if (aviso == null)
                    return NotFound(new { mensaje = "Aviso no encontrado" });

                return Ok(aviso);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el aviso", error = ex.Message });
            }
        }

        //Editar aviso
        [HttpPut("EditarAviso")]
        public async Task<IActionResult> EditarAviso([FromBody] tbAvisos model)
        {
            try
            {
                var aviso = await _context.tbAvisos.FindAsync(model.AvisoId);
                if (aviso == null)
                    return NotFound(new { mensaje = "Aviso no encontrado" });

                aviso.Titulo = model.Titulo;
                aviso.Descripcion = model.Descripcion;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Aviso actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el aviso", error = ex.Message });
            }
        }


        // Obtener solo ID y Nombre de las actividades por materia
        [HttpGet("ObtenerActividadParaSelect/{materiaId}")]
        public async Task<IActionResult> ObtenerActividadesSimples(int materiaId)
        {
            var actividades = await _context.tbActividades
                .Where(a => a.MateriaId == materiaId)
                .Select(a => new { a.ActividadId, a.NombreActividad }) // Solo ID y Nombre
                .ToListAsync();

            return Ok(actividades);
        }




    }
}
