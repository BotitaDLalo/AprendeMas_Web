using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosAgendaController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public EventosAgendaController(DataContext dataContext) { 
            _dataContext = dataContext;
        }

        public async Task<List<object>> ConsultaEventos()
        {
            try
            {
                // Consultamos los eventos de la agenda
                var eventos = await _dataContext.tbEventosAgenda
                    .Include(e => e.EventosGrupos)  // Incluimos los eventos de grupos
                    .Include(e => e.EventosMaterias) // Incluimos los eventos de materias
                    .ToListAsync();

                // Formateamos los eventos para devolver solo la información necesaria
                var listaEventos = eventos.Select(e => new
                {
                    e.EventoId,
                    e.DocenteId,
                    e.Titulo,
                    e.Descripcion,
                    e.Color,
                    FechaInicio = e.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    FechaFinal = e.FechaFinal.ToString("yyyy-MM-ddTHH:mm:ss"),
                    e.EventosGrupos?.FirstOrDefault()?.GrupoId,
                    e.EventosMaterias?.FirstOrDefault()?.MateriaId,
                }).ToList();

                return listaEventos.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                // En caso de error, lanzamos la excepción para ser manejada en el controlador
                throw new Exception("Hubo un problema al consultar los eventos", ex);
            }
        }



        [HttpGet("ObtenerEventos")]
        public async Task<ActionResult<List<object>>> ObtenerEventos(int docenteId)
        {
            try
            {
                // Consulta eventos y convierte la lista a dynamic
                var lsEventos = await ConsultaEventos();
                var eventosDinamicos = lsEventos.Cast<dynamic>();

                // Filtrar los eventos por docenteId
                var eventosFiltrados = eventosDinamicos
                    .Where(e => e.DocenteId == docenteId)
                    .ToList();

                // Devolver los eventos filtrados
                return Ok(eventosFiltrados);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }


        [HttpPost("CrearEventos")]
        public async Task<ActionResult<EventosAgenda>> CrearEvento([FromBody] EventosAgenda nuevoEvento)
        {

            try
            {
                // Primero, guardar el evento en la tabla EventosAgenda
                _dataContext.tbEventosAgenda.Add(nuevoEvento);
                await _dataContext.SaveChangesAsync();  // Esto genera automáticamente el EventoId (FechaId)

                // Después de guardar, asignamos el EventoId generado a EventosGrupos y EventosMaterias
                if (nuevoEvento.EventosGrupos != null)
                {
                    var grupos = nuevoEvento.EventosGrupos.Select(grupo => new EventosGrupos
                    {
                        FechaId = nuevoEvento.EventoId,
                        GrupoId = grupo.GrupoId
                    }).ToList();

                    _dataContext.tbEventosGrupos.AddRange(grupos);

                }

                if (nuevoEvento.EventosMaterias != null)
                {
                    var materias = nuevoEvento.EventosMaterias.Select(materia => new EventosMaterias
                    {
                        FechaId = nuevoEvento.EventoId,
                        MateriaId = materia.MateriaId,
                    }).ToList();

                    _dataContext.tbEventosMaterias.AddRange(materias);
                }

                await _dataContext.SaveChangesAsync(); // Guardamos las relaciones en EventosGrupos y EventosMaterias

                // Retornamos el evento creado, o el ID generado para confirmación
                return Ok(new { Message = "Evento creado correctamente." });
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }

        }


        [HttpPut("ActualizarEvento/{id}")]
        public async Task<ActionResult> ActualizarEvento(int id, [FromBody] EventosAgenda eventoActualizado)
        {
            try
            {
                var eventoExistente = await _dataContext.tbEventosAgenda
                    .Include(e => e.EventosGrupos)
                    .Include(e => e.EventosMaterias)
                    .FirstOrDefaultAsync(e => e.EventoId == id);

                if (eventoExistente == null)
                {
                    return NotFound(new { Message = "Evento no encontrado." });
                }

                // Actualizar los campos básicos del evento
                eventoExistente.Titulo = eventoActualizado.Titulo;
                eventoExistente.Descripcion = eventoActualizado.Descripcion;
                eventoExistente.Color = eventoActualizado.Color;
                eventoExistente.FechaInicio = eventoActualizado.FechaInicio;
                eventoExistente.FechaFinal = eventoActualizado.FechaFinal;

                // **Eliminar relaciones anteriores solo si se enviaron nuevas**
                if ((eventoActualizado.EventosGrupos != null && eventoActualizado.EventosGrupos.Any()) ||
                    (eventoActualizado.EventosMaterias != null && eventoActualizado.EventosMaterias.Any()))
                {
                    _dataContext.tbEventosGrupos.RemoveRange(eventoExistente.EventosGrupos);
                    _dataContext.tbEventosMaterias.RemoveRange(eventoExistente.EventosMaterias);
                }

                // **Validar que los grupos existan antes de insertarlos**
                if (eventoActualizado.EventosGrupos != null && eventoActualizado.EventosGrupos.Any())
                {
                    var grupoIds = eventoActualizado.EventosGrupos.Select(g => g.GrupoId).ToList();
                    var gruposExistentes = await _dataContext.tbGrupos
                        .Where(g => grupoIds.Contains(g.GrupoId))
                        .Select(g => g.GrupoId)
                        .ToListAsync();

                    if (gruposExistentes.Count != grupoIds.Count)
                    {
                        return BadRequest(new { Message = "Uno o más GrupoId no existen en la base de datos." });
                    }

                    eventoExistente.EventosGrupos = eventoActualizado.EventosGrupos.Select(grupo => new EventosGrupos
                    {
                        FechaId = eventoExistente.EventoId,
                        GrupoId = grupo.GrupoId
                    }).ToList();
                }

                // **Validar que las materias existan antes de insertarlas**
                if (eventoActualizado.EventosMaterias != null && eventoActualizado.EventosMaterias.Any())
                {
                    var materiaIds = eventoActualizado.EventosMaterias.Select(m => m.MateriaId).ToList();
                    var materiasExistentes = await _dataContext.tbMaterias
                        .Where(m => materiaIds.Contains(m.MateriaId))
                        .Select(m => m.MateriaId)
                        .ToListAsync();

                    if (materiasExistentes.Count != materiaIds.Count)
                    {
                        return BadRequest(new { Message = "Uno o más MateriaId no existen en la base de datos." });
                    }

                    eventoExistente.EventosMaterias = eventoActualizado.EventosMaterias.Select(materia => new EventosMaterias
                    {
                        FechaId = eventoExistente.EventoId,
                        MateriaId = materia.MateriaId
                    }).ToList();
                }

                // Si no hay grupos ni materias, lanzar un error
                if ((eventoActualizado.EventosGrupos == null || !eventoActualizado.EventosGrupos.Any()) &&
                    (eventoActualizado.EventosMaterias == null || !eventoActualizado.EventosMaterias.Any()))
                {
                    return BadRequest(new { Message = "El evento debe estar asignado a al menos un grupo o materia." });
                }

                // Guardar cambios en la base de datos
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "Evento actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocurrió un error al actualizar el evento.", Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }



        //[HttpPut("ActualizarEventos/{id}")]
        //public async Task<ActionResult<EventosAgenda>> ActualizarEvento(int id, EventosAgenda actEvento)
        //{
        //    try
        //    {
        //        // Buscar el evento en la base de datos
        //        var dbEventoAgenda = await _dataContext.tbEventosAgenda
        //            .Include(e => e.EventosGrupos) // Incluir relaciones con grupos
        //            .Include(e => e.EventosMaterias) // Incluir relaciones con materias
        //            .FirstOrDefaultAsync(e => e.EventoId == id);

        //        if (dbEventoAgenda is null)
        //            return NotFound("Evento no encontrado");

        //        // Actualizar los campos del evento
        //        dbEventoAgenda.Titulo = actEvento.Titulo;
        //        dbEventoAgenda.Descripcion = actEvento.Descripcion;
        //        dbEventoAgenda.FechaInicio = actEvento.FechaInicio;
        //        dbEventoAgenda.FechaFinal = actEvento.FechaFinal;
        //        dbEventoAgenda.Color = actEvento.Color;

        //        // Eliminar relaciones existentes solo para este evento
        //        if (dbEventoAgenda.EventosGrupos.Any())
        //        {
        //            var gruposARemover = dbEventoAgenda.EventosGrupos
        //                .Where(eg => eg.FechaId == id)
        //                .ToList();

        //            _dataContext.tbEventosGrupos.RemoveRange(gruposARemover);
        //        }

        //        if (dbEventoAgenda.EventosMaterias.Any())
        //        {
        //            var materiasARemover = dbEventoAgenda.EventosMaterias
        //                .Where(em => em.FechaId == id)
        //                .ToList();

        //            _dataContext.tbEventosMaterias.RemoveRange(materiasARemover);
        //        }

        //        // Agregar nuevas relaciones si se proporcionan
        //        if (actEvento.EventosGrupos != null && actEvento.EventosGrupos.Any())
        //        {
        //            foreach (var grupo in actEvento.EventosGrupos)
        //            {
        //                var nuevoGrupo = new EventosGrupos
        //                {
        //                    FechaId = id, // Asocia el id del evento actual
        //                    GrupoId = grupo.GrupoId
        //                };
        //                _dataContext.tbEventosGrupos.Add(nuevoGrupo);
        //            }
        //        }

        //        if (actEvento.EventosMaterias != null && actEvento.EventosMaterias.Any())
        //        {
        //            foreach (var materia in actEvento.EventosMaterias)
        //            {
        //                var nuevaMateria = new EventosMaterias
        //                {
        //                    FechaId = id, // Asocia el id del evento actual
        //                    MateriaId = materia.MateriaId
        //                };
        //                _dataContext.tbEventosMaterias.Add(nuevaMateria);
        //            }
        //        }

        //        // Guardar los cambios en la base de datos
        //        await _dataContext.SaveChangesAsync();

        //        // Retornar el evento actualizado
        //        return Ok(dbEventoAgenda);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error inesperado: {ex.Message}");
        //    }
        //}



        [HttpDelete("EliminarEvento")]
        public async Task<ActionResult> EliminarEvento([FromQuery] int eventoId, [FromQuery] int docenteId)
        {
            try
            {
                // Buscar el evento en la tabla EventosAgenda
                var evento = await _dataContext.tbEventosAgenda
                    .Include(e => e.EventosGrupos)
                    .Include(e => e.EventosMaterias)
                    .FirstOrDefaultAsync(e => e.EventoId == eventoId);

                if (evento == null)
                {
                    return NotFound(new { Message = "El evento no fue encontrado." });
                }

                // Eliminar las relaciones en EventosGrupos si existen
                if (evento.EventosGrupos != null && evento.EventosGrupos.Any())
                {
                    _dataContext.tbEventosGrupos.RemoveRange(evento.EventosGrupos);
                }

                // Eliminar las relaciones en EventosMaterias si existen
                if (evento.EventosMaterias != null && evento.EventosMaterias.Any())
                {
                    _dataContext.tbEventosMaterias.RemoveRange(evento.EventosMaterias);
                }

                // Eliminar el evento de la tabla EventosAgenda
                _dataContext.tbEventosAgenda.Remove(evento);

                // Guardar los cambios en la base de datos
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "Evento eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocurrió un error al eliminar el evento.", Error = ex.Message });
            }
        }


    }
}
