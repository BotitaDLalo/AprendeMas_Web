using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriasApiController : ControllerBase
    {
        private readonly DataContext _context;

        public MateriasApiController(DataContext context)
        {
            _context = context;
        }

        //Aqui se creara la materia.
        [HttpPost("CrearMateria")]
        public async Task<IActionResult> CrearMateria([FromBody] Materias materia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos de materia invalido.");
            }

            materia.CodigoAcceso = ObtenerClaveMateria();
            _context.tbMaterias.Add(materia);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Materia creada con exito.", materiaId = materia.MateriaId });
        }

        //Genera una clave para la materia
        private string ObtenerClaveMateria()
        {
            var random = new Random();
            return new string(Enumerable.Range(0, 10).Select(_ => (char)random.Next('A', 'Z')).ToArray());
        }

        //Generamos el get para obtener las materias que no tienen asignado un grupo
        [HttpGet("ObtenerMateriasSinGrupo/{docenteId}")]
        public async Task<IActionResult> ObtenerMateriasSinGrupo(int docenteId)
        {
            var materias = await _context.tbMaterias
                .Where(g => g.DocenteId == docenteId)
                .ToListAsync();
            return Ok(materias);
        }

    }
}
