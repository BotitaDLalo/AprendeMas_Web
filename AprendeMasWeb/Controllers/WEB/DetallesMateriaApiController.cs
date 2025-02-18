using AprendeMasWeb.Data;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers.WEB
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallesMateriaApiController : ControllerBase
    {
        private readonly DataContext _context;

        public DetallesMateriaApiController(DataContext context)
        {
            _context = context;
        }


        //Generamos el get donde obtendremos los datos de una  materia especifica para mostrar en vista.
        [HttpGet("ObtenerDetallesMateria/{materiaId}/{docenteId}")]
        public async Task<IActionResult> ObtenerDetallesMateria(int materiaId, int docenteId)
        {
            var materiaDetalles = await _context.tbMaterias
                .Where(m => m.MateriaId == materiaId && m.DocenteId == docenteId) // Filtro por docente
                .Select(m => new
                {
                    NombreMateria = m.NombreMateria,
                    CodigoAcceso = m.CodigoAcceso,
                    CodigoColor = m.CodigoColor,
                    DocenteId = m.DocenteId

                }).FirstOrDefaultAsync();

            if (materiaDetalles == null) // se remplaza por lo que es id del docente actual.
            {

                return NotFound(new { mensaje = "Materia No Encontrada O Sin Permiso" });
            }

            return Ok(materiaDetalles);

        }
    }
}


