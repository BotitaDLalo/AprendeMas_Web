using AprendeMasWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly DataContext _context;

        public AlumnosController(DataContext context)
        {
            _context = context;
        }

        public ActionResult RegistrarAlumnoGrupoCodigo([FromBody] AlumnoGrupoRegistro alumnoGrupoRegistro)
        {
            try
            {
                int alumnoId = alumnoGrupoRegistro.AlumnoId;
                string codigoAcceso = alumnoGrupoRegistro.CodigoAcceso;


                var grupoId = _context.tbGrupos.Where(a => a.CodigoAcceso == codigoAcceso).Select(a=>a.GrupoId).FirstOrDefault();

                AlumnosGrupos alumnoGrupo = new()
                {
                    AlumnoId = alumnoId,
                    GrupoId = grupoId,
                };

                _context.tbAlumnosGrupos.Add(alumnoGrupo);
                _context.SaveChanges();

                return Ok();
            }catch (Exception e)
            {
                return BadRequest(new {mensaje = e.Message});
            }
        }



        public async Task<ActionResult> RegistrarAlumnoMateriaCodigo([FromBody] AlumnoGrupoRegistro alumnoGrupoRegistro)
        {
            try
            {
                int alumnoId = alumnoGrupoRegistro.AlumnoId;
                string codigoAcceso = alumnoGrupoRegistro.CodigoAcceso;


                var materiaId = _context.tbMaterias.Where(a => a.CodigoAcceso == codigoAcceso).Select(a => a.GrupoId).FirstOrDefault();

                AlumnosMaterias alumnoGrupo = new()
                {
                    AlumnoId = alumnoId,
                    MateriaId = materiaId
                };

                _context.tbAlumnosMaterias.Add(alumnoGrupo);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
        }
    }
}
