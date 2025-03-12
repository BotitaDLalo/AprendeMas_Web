// Se importan los espacios de nombres necesarios para trabajar con la base de datos y las API de ASP.NET Core
using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    // Se define la ruta base para este controlador API (utilizando la convención de API REST)
    [Route("api/[controller]")]
    // Indica que este controlador es para una API
    [ApiController]
    public class GruposApiController : ControllerBase
    {
        // Se declara el contexto de la base de datos para interactuar con los datos de la aplicación
        private readonly DataContext _context;

        // Constructor que recibe el contexto de datos para poder interactuar con la base de datos
        public GruposApiController(DataContext context)
        {
            _context = context; // Asigna el contexto de datos a la variable de la clase
        }

        // Acción para crear un grupo mediante una solicitud POST (API)
        [HttpPost("CrearGrupo")]
        public async Task<IActionResult> CrearGrupo([FromBody] Grupos grupo)
        {
            // Verifica si el modelo enviado es válido (ejemplo: los datos del grupo están completos)
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, devuelve un mensaje de error con un estado BadRequest
                return BadRequest("Datos del grupo invalidos.");
            }

            // Genera un código de acceso para el grupo
            grupo.CodigoAcceso = ObtenerClaveGrupo();
            // Agrega el grupo a la base de datos
            _context.tbGrupos.Add(grupo);
            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();
            // Retorna una respuesta exitosa con un mensaje y el ID del grupo creado
            return Ok(new { mensaje = "Grupo creado con exito.", grupoId = grupo.GrupoId });
        }

        // Método privado que genera una clave aleatoria de 8 caracteres para el grupo
        private string ObtenerClaveGrupo()
        {
            var random = new Random(); // Crea una instancia de la clase Random
            // Genera una cadena de 8 caracteres aleatorios entre A y Z
            return new string(Enumerable.Range(0, 8).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }

        // Acción para obtener los grupos pertenecientes a un docente específico
        [HttpGet("ObtenerGrupos/{docenteId}")]
        public async Task<IActionResult> ObtenerGrupos(int docenteId)
        {
            // Consulta los grupos en la base de datos que pertenecen al docente con el ID proporcionado
            var grupos = await _context.tbGrupos
                .Where(g => g.DocenteId == docenteId)
                .ToListAsync();
            // Devuelve una respuesta exitosa con los grupos obtenidos
            return Ok(grupos);
        }

        // Acción POST para asociar materias a un grupo
        [HttpPost("AsociarMaterias")]
        public async Task<IActionResult> AsociarMaterias([FromBody] AsociarMateriasRequest request)
        {
            // Verifica que los datos de la solicitud no sean nulos y que contengan al menos una materia
            if (request == null || request.MateriaIds == null || request.MateriaIds.Count == 0)
            {
                return BadRequest("Datos Invalidos"); // Retorna un error si los datos son inválidos
            }

            // Verifica si el grupo existe en la base de datos
            var grupo = await _context.tbGrupos.FindAsync(request.GrupoId);
            if (grupo == null)
            {
                return NotFound("Grupo no encontrado."); // Si no se encuentra el grupo, retorna un error
            }

            // Elimina las asociaciones previas entre el grupo y las materias (opcional, si quieres reemplazar en lugar de agregar nuevas)
            var asociacionesActuales = _context.tbGruposMaterias.Where(gm => gm.GrupoId == request.GrupoId);
            _context.tbGruposMaterias.RemoveRange(asociacionesActuales);

            // Asocia las materias seleccionadas al grupo
            foreach (var materiaId in request.MateriaIds)
            {
                // Verifica si la materia existe en la base de datos
                var materiaExiste = await _context.tbMaterias.AnyAsync(m => m.MateriaId == materiaId);
                if (materiaExiste)
                {
                    // Crea una nueva relación entre el grupo y la materia
                    var nuevaRelacion = new GruposMaterias
                    {
                        GrupoId = request.GrupoId,
                        MateriaId = materiaId
                    };
                    // Agrega la nueva relación a la base de datos
                    _context.tbGruposMaterias.Add(nuevaRelacion);
                }
            }

            // Guarda los cambios en la base de datos
            await _context.SaveChangesAsync();
            // Retorna una respuesta exitosa con un mensaje
            return Ok(new { mensaje = "Materias asociadas correctamente." });
        }

        [HttpGet("ObtenerMateriasPorGrupo/{grupoId}")]
        public async Task<IActionResult> ObtenerMateriasPorGrupo(int grupoId)
        {
            var materiasIds = await _context.tbGruposMaterias
                .Where(gm => gm.GrupoId == grupoId)
                .Select(gm => gm.MateriaId)
                .ToListAsync();

            var materias = await _context.tbMaterias
                .Where(m => materiasIds.Contains(m.MateriaId))
                .Select(m => new
                {
                    m.MateriaId,
                    m.NombreMateria,
                    m.Descripcion,
                    m.CodigoColor
                })
                .ToListAsync();

            return Ok(materias);
        }

        [HttpDelete("EliminarGrupo/{grupoId}")]
        public async Task<IActionResult> EliminarGrupo(int grupoId)
        {
            //buscar si el grupo existe en la base de datos
            var grupo = await _context.tbGrupos.FindAsync(grupoId);
            if(grupo == null)
            {
                return NotFound(new { mensaje = "El grupo no existe." });
            }

            //Eliminar todas las relaciones del grupo en la tabla GruposYMaterias
            var relaciones = _context.tbGruposMaterias
                .Where(gm => gm.GrupoId == grupoId);
            _context.tbGruposMaterias.RemoveRange(relaciones);

            //Eliminar el grupo despues de haber eliminado las relaciones 
            _context.tbGrupos.Remove(grupo);

            //Guardamos los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Grupo eliminado correctamente." });
        }

        //Elimina grupo con materias
        [HttpDelete("EliminarGrupoConMaterias/{grupoId}")]
        public async Task<IActionResult> EliminarGrupoConMaterias(int grupoId)
        {
            // Buscar el grupo en la base de datos
            var grupo = await _context.tbGrupos.FindAsync(grupoId);
            if (grupo == null)
            {
                return NotFound(new { mensaje = "El grupo no existe" });
            }

            // Buscar las relaciones en GruposMaterias
            var relacionesGruposMaterias = _context.tbGruposMaterias.Where(mg => mg.GrupoId == grupoId);

            // Obtener los IDs de las materias asociadas al grupo
            var materiasIds = relacionesGruposMaterias.Select(r => r.MateriaId).ToList();

            // Buscar y eliminar las relaciones en AlumnosMaterias
            var relacionesAlumnosMaterias = _context.tbAlumnosMaterias.Where(am => materiasIds.Contains(am.MateriaId));
            _context.tbAlumnosMaterias.RemoveRange(relacionesAlumnosMaterias);

            // Eliminar todas las relaciones de la materia con grupos
            _context.tbGruposMaterias.RemoveRange(relacionesGruposMaterias);

            // Buscar las materias asociadas a este grupo
            var materias = _context.tbMaterias.Where(m => materiasIds.Contains(m.MateriaId));

            // Eliminar todas las actividades relacionadas con estas materias
            var relacionesMateriasActividades = _context.tbMateriasActividades.Where(ma => materiasIds.Contains(ma.MateriaId));
            _context.tbMateriasActividades.RemoveRange(relacionesMateriasActividades);

            // Obtener los IDs de las actividades relacionadas
            var actividadesIds = relacionesMateriasActividades.Select(ma => ma.ActividadId).ToList();

            // Eliminar las actividades asociadas a estas materias
            var actividades = _context.tbActividades.Where(a => actividadesIds.Contains(a.ActividadId));
            _context.tbActividades.RemoveRange(actividades);

            // Eliminar las materias
            _context.tbMaterias.RemoveRange(materias);

            // Eliminar el grupo
            _context.tbGrupos.Remove(grupo);

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Grupo, materias y actividades eliminados correctamente" });
        }



    }
}

// Modelo para recibir la solicitud desde el frontend (para asociar materias a un grupo)
public class AsociarMateriasRequest
{
    public int GrupoId { get; set; } // ID del grupo
    public List<int> MateriaIds { get; set; } // Lista de IDs de las materias que se asociarán al grupo
}
