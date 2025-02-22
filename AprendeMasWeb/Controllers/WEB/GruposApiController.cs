using AprendeMasWeb.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Route("api/[controller]")]
    [ApiController]
    public class GruposApiController : ControllerBase
    {
        private readonly DataContext _context;
        public GruposApiController(DataContext context)
        {
            _context = context;

        }

        //Aqui se crea el grupo a traves de API peticion post 
        [HttpPost("CrearGrupo")]
        public async Task<IActionResult> CrearGrupo([FromBody] Grupos grupo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos del grupo invalidos.");
            }

            grupo.CodigoAcceso = ObtenerClaveGrupo();
            _context.tbGrupos.Add(grupo);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Grupo creado con exito.", grupoId = grupo.GrupoId });
        }

        //Genera una clave para el grupo
        private string ObtenerClaveGrupo()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 8).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }


        //Controlador que ayuda obtener los grupos que estan en la base de datos que le pertenecen al docente que inicia sesion.
        [HttpGet("ObtenerGrupos/{docenteId}")]
        public async Task<IActionResult> ObtenerGrupos(int docenteId)
        {
            var grupos = await _context.tbGrupos
                .Where(g => g.DocenteId == docenteId)
                .ToListAsync();
            return Ok(grupos);
        }


        // peticion api para registrar a la base de datos el enlace de un grupo con una materia.
        [HttpPost("AsociarMaterias")]
        public async Task<IActionResult> AsociarMaterias([FromBody] AsociarMateriasRequest request)
        {
            if(request == null || request.MateriaIds == null || request.MateriaIds.Count == 0)
            {
                return BadRequest("Datos Invalidos");
            }

            // Verifica si el grupo existe
            var grupo = await _context.tbGrupos.FindAsync(request.GrupoId);
            if (grupo == null)
            {
                return NotFound("Grupo no encontrado.");
            }

            // Eliminar asociaciones previas del grupo (opcional, si quieres reemplazar en lugar de agregar)
            var asociacionesActuales = _context.tbGruposMaterias.Where(gm => gm.GrupoId == request.GrupoId);
            _context.tbGruposMaterias.RemoveRange(asociacionesActuales);

            // Asociar las materias seleccionadas al grupo
            foreach (var materiaId in request.MateriaIds)
            {
                var materiaExiste = await _context.tbMaterias.AnyAsync(m => m.MateriaId == materiaId);
                if (materiaExiste)
                {
                    var nuevaRelacion = new GruposMaterias
                    {
                        GrupoId = request.GrupoId,
                        MateriaId = materiaId
                    };
                    _context.tbGruposMaterias.Add(nuevaRelacion);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Materias asociadas correctamente." });

        }
    }
}

// Modelo para recibir la solicitud desde el frontend
public class AsociarMateriasRequest
{
    public int GrupoId { get; set; }
    public List<int> MateriaIds { get; set; }
}