﻿using AprendeMasWeb.Data;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Models;
using Microsoft.EntityFrameworkCore;
using AprendeMasWeb.Models.DBModels;
using AprendeMasWeb.Services;
using System.Linq;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.AccessControl;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadesController(UserManager<IdentityUser> userManager, DataContext context, ITiposActividadesService tiposActividadesService) : ControllerBase
    {
        private readonly DataContext _context = context;
        private readonly ITiposActividadesService _tiposActividadesService = tiposActividadesService;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        // Cambiar el tipo de retorno a ActionResult<List<object>> para que pueda ser usado en respuestas HTTP
        public async Task<List<object>> ConsultaActividades()
        {
            try
            {
                var listaActividades = await _context.tbActividades
                    .Select(a => new
                    {
                        actividadId = a.ActividadId,
                        nombreActividad = a.NombreActividad,
                        descripcionActividad = a.Descripcion,
                        fechaCreacionActividad = a.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
                        fechaLimiteActividad = a.FechaLimite.ToString("yyyy-MM-ddTHH:mm:ss"),
                        tipoActividadId = a.TipoActividadId,
                        puntaje = a.Puntaje,
                        materiaId = a.MateriaId
                    })
                    .ToListAsync();

                return listaActividades.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                return [];
            }
        }



        // Cambiar el tipo de retorno a ActionResult<List<object>> para ser consistente
        public async Task<ActionResult<List<object>>> ConsultarActividadesCreadas()
        {
            try
            {
                var lsActividades = await _context.tbActividades.Select(a => new
                {
                    a.ActividadId,
                    a.NombreActividad
                }).ToListAsync();

                return Ok(lsActividades); // Retorna la lista de actividades creadas
            }
            catch (Exception)
            {
                return BadRequest("Ocurrió un error al obtener las actividades creadas.");
            }
        }

        public async Task<ActionResult<List<object>>> ConsultaActividadesPorMateria(int materiaId)
        {
            try
            {
                var listaActividades = await _context.tbActividades
                    .Where(a => a.MateriaId == materiaId)
                    .Select(a => new
                    {
                        actividadId = a.ActividadId,
                        nombreActividad = a.NombreActividad,
                        descripcionActividad = a.Descripcion,
                        fechaCreacionActividad = a.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
                        fechaLimiteActividad = a.FechaLimite.ToString("yyyy-MM-ddTHH:mm:ss"),
                        tipoActividadId = a.TipoActividadId,
                        puntaje = a.Puntaje,
                        materiaId = a.MateriaId
                    })
                    .ToListAsync();

                return Ok(listaActividades);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error al obtener las actividades para la materia {materiaId}: {ex.Message}");
            }
        }


        [HttpGet("ObtenerActividadesPorMateria/{materiaId}")]
        public async Task<ActionResult<List<object>>> ObtenerActividadesPorMateria(int materiaId)
        {

            try
            {
                var lsActividades = await ConsultaActividadesPorMateria(materiaId);
                return lsActividades;
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }

        }



        // El tipo de retorno debe ser ActionResult<List<object>> porque estamos devolviendo una lista de objetos
        [HttpGet("ObtenerActividades")]
        public async Task<ActionResult<List<object>>> ObtenerActividades()
        {
            try
            {
                var lsActividades = await ConsultaActividades();

                return Ok(lsActividades); // Retorna la lista obtenida de ConsultaActividades
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message }); // En caso de error, retornamos el mensaje de la excepción
            }
        }



        // Obtener una actividad específica
        [HttpGet("{id}")]
        public async Task<ActionResult<tbActividades>> ObtenerActividad(int id)
        {
            var activity = await _context.tbActividades.FindAsync(id);
            if (activity == null) return NotFound("Actividad no encontrada"); // Retorna un mensaje adecuado si no se encuentra la actividad

            return Ok(activity); // Si la actividad se encuentra, la retornamos
        }

        [HttpPost("CrearActividad")]
        public async Task<ActionResult<List<tbActividades>>> CrearActividad([FromBody] tbActividades nuevaActividad)
        {
            try
            {
                int materiaId = nuevaActividad.MateriaId;
                // Verificar si la materia existe
                var materia = await _context.tbMaterias.FindAsync(materiaId);
                if (materia == null)
                {
                    return BadRequest("La materia asociada no existe.");
                }

                // Validar campos no nulos o con valores incorrectos
                if (string.IsNullOrWhiteSpace(nuevaActividad.NombreActividad))
                {
                    return BadRequest("El nombre de la actividad es obligatorio.");
                }

                if (nuevaActividad.FechaLimite == default(DateTime))
                {
                    return BadRequest("La fecha límite de la actividad es inválida.");
                }

                // Generar automáticamente la fecha de creación
                nuevaActividad.FechaCreacion = DateTime.Now;

                // Obtener o crear el tipo de actividad si no se especifica
                if (nuevaActividad.TipoActividadId == 0)
                {
                    var tipoActividad = await _tiposActividadesService.GetOrCreateTipoActividad(1); // Por defecto, 1 es "Actividad"
                    nuevaActividad.TipoActividadId = tipoActividad.TipoActividadId;
                }

                // Guardar la actividad en la base de datos
                _context.tbActividades.Add(nuevaActividad);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Actividad creada con éxito", actividadId = nuevaActividad.ActividadId });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al actualizar la base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }



        [HttpPut("ActualizarActividad/{id}")]
        public async Task<ActionResult<tbActividades>> UpdateActivity(int id, tbActividades updatedActivity)
        {
            var dbActivity = await _context.tbActividades.FindAsync(id);
            if (dbActivity is null) return NotFound("Actividad no encontrada");

            dbActivity.NombreActividad = updatedActivity.NombreActividad;
            dbActivity.Descripcion = updatedActivity.Descripcion;
            dbActivity.FechaLimite = updatedActivity.FechaLimite;

            await _context.SaveChangesAsync();
            return Ok(dbActivity); // Retorna solo la actividad actualizada
        }


        [HttpDelete("EliminarActividad/{id}")]
        public async Task<ActionResult> DeleteActivity(int id)
        {
            try
            {
                var activity = await _context.tbActividades.FirstOrDefaultAsync(a => a.ActividadId == id);

                if (activity is null) return BadRequest("Actividad no encontrada");

                var alumnoActividad = await _context.tbAlumnosActividades.FirstOrDefaultAsync(a => a.ActividadId == activity.ActividadId);

                if (alumnoActividad != null)
                {

                    var entrega = await _context.tbEntregablesAlumno.Where(a => a.AlumnoActividadId == alumnoActividad.AlumnoActividadId).FirstOrDefaultAsync();
                    if (entrega != null)
                    {
                        var calificacion = await _context.tbCalificaciones.FirstOrDefaultAsync(a => a.EntregaId == entrega.EntregaId);

                        if (calificacion != null)
                        {
                            _context.tbCalificaciones.Remove(calificacion);
                            _context.tbEntregablesAlumno.Remove(entrega);
                            _context.tbAlumnosActividades.Remove(alumnoActividad);
                        }
                        else
                        {
                            _context.tbEntregablesAlumno.Remove(entrega);
                            _context.tbAlumnosActividades.Remove(alumnoActividad);
                        }
                    }
                }

                _context.tbActividades.Remove(activity);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al actualizar la base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }



        [HttpGet("ObtenerAlumnosEntregables")]
        public async Task<ActionResult<RespuestaAlumnosEntregables>> ObtenerAlumnosEntregables(int actividadId)
        {
            try
            {
                List<AlumnoEntregable> lsEntregables = new List<AlumnoEntregable>();
                RespuestaAlumnosEntregables respuestaAlumnos = new RespuestaAlumnosEntregables();

                var lsAlumnosActividades = await _context.tbAlumnosActividades
                    .Where(a => a.ActividadId == actividadId && a.EstatusEntrega == true)
                    .Include(a => a.EntregablesAlumno)
                    .Include(a => a.Actividades)
                    .Include(a => a.Alumnos).ToListAsync();


                int puntaje = await _context.tbActividades.Where(a => a.ActividadId == actividadId).Select(a => a.Puntaje).FirstOrDefaultAsync();

                int totalEntregados = lsAlumnosActividades.Count;

                respuestaAlumnos.ActividadId = actividadId;
                respuestaAlumnos.Puntaje = puntaje;
                respuestaAlumnos.TotalEntregados = totalEntregados;

                foreach (var alumnoActividad in lsAlumnosActividades)
                {
                    AlumnoEntregable alumnoEntregable = new AlumnoEntregable();
                    var alumno = alumnoActividad.Alumnos;
                    var entregableAlumno = alumnoActividad.EntregablesAlumno;
                    if (alumno != null && entregableAlumno != null)
                    {
                        var entregaId = entregableAlumno.EntregaId;

                        var alumnoId = alumno.AlumnoId;
                        var userId = alumno.UserId;
                        var nombres = alumno.Nombre;
                        var apellidoPaterno = alumno.ApellidoPaterno;
                        var apellidoMaterno = alumno.ApellidoMaterno;
                        var user = await _userManager.FindByIdAsync(userId ?? "");

                        if (user != null)
                        {
                            var userName = user.UserName;
                            alumnoEntregable.AlumnoId = alumnoId;
                            alumnoEntregable.NombreUsuario = userName ?? "";
                            alumnoEntregable.Nombres = nombres ?? "";
                            alumnoEntregable.ApellidoPaterno = apellidoPaterno ?? "";
                            alumnoEntregable.ApellidoMaterno = apellidoMaterno ?? "";
                        }

                        alumnoEntregable.FechaEntrega = alumnoActividad.FechaEntrega;

                        alumnoEntregable.EntregaId = entregableAlumno.EntregaId;
                        alumnoEntregable.Respuesta = entregableAlumno.Respuesta ?? "";

                        var calificacion = await _context.tbCalificaciones.Where(a => a.EntregaId == entregaId).FirstOrDefaultAsync();

                        alumnoEntregable.Calificacion = calificacion?.Calificacion ?? -1;

                        lsEntregables.Add(alumnoEntregable);
                    }

                }

                respuestaAlumnos.AlumnosEntregables = lsEntregables;


                return Ok(respuestaAlumnos);
            }
            catch (Exception e)
            {
                return BadRequest($"Error: {e.Message}");
            }
        }


        [HttpPost("AsignarCalificacion")]
        public async Task<ActionResult> AsignarCalificacion([FromBody] AsignarCalificacionPeticion asignarCalificacion)
        {
            try
            {
                var entregaId = asignarCalificacion.EntregaId;
                var fechaNuevaCalificacion = DateTime.Now;
                var nuevaCalificacion = asignarCalificacion.Calificacion;

                var calificacion = await _context.tbCalificaciones.Where(a => a.EntregaId == entregaId).FirstOrDefaultAsync();

                if (calificacion == null)
                {
                    tbCalificaciones calificaciones = new tbCalificaciones()
                    {
                        Calificacion = nuevaCalificacion,
                        EntregaId = entregaId,
                        FechaCalificacionAsignada = fechaNuevaCalificacion
                    };

                    await _context.tbCalificaciones.AddAsync(calificaciones);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    calificacion.Calificacion = nuevaCalificacion;
                    calificacion.FechaCalificacionAsignada = fechaNuevaCalificacion;
                    _context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}