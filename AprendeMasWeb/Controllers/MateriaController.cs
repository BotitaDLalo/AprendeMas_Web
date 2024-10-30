using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaController : Controller
    {
        private readonly DataContext _context;

        public MateriaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MateriaRegistro>>> GetAllSubjects()
        {
            var subjects = await _context.Materias.ToListAsync();

            return subjects;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaRegistro>> GetSubjects(int id)
        {
            var subject = await _context.Materias.FindAsync(id);
            if (subject is null) return NotFound("Materia no encontrado");

            return Ok(subject);
        }

        [HttpPost("MateriaSinGrupo")]
        public async Task<ActionResult> CrearMateriaSinGrupo([FromBody] MateriaRegistro materia)
        {
            try
            {
                _context.Materias.Add(materia);
                await _context.SaveChangesAsync();
                return Ok(await _context.Materias.ToListAsync());
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "No se registro la materia" });
            }
        }

        [HttpPost("MateriaConGrupo")]
        public async Task<ActionResult<List<MateriaConGrupo>>> CrearMateriaGrupos([FromBody] MateriaConGrupo materiaG)
        {
            try
            {
                MateriaRegistro materia = new()
                {
                    NombreMateria = materiaG.NombreMateria,
                    Descripcion = materiaG.Descripcion,
                    CodigoColor = materiaG.CodigoColor,
                };


                // Agrega la materia al contexto
                _context.Materias.Add(materia);
                // Guarda los cambios para obtener el ID generado
                await _context.SaveChangesAsync();

                var idMateria = materia.MateriaId;
                List<int> gruposVinculados = materiaG.GruposVinculados;

                foreach (var grupo in gruposVinculados)
                {
                    GruposMaterias gruposMaterias = new()
                    {
                        GrupoId = grupo,
                        MateriaId = idMateria
                    };

                    _context.GruposMaterias.Add(gruposMaterias);
                }
                await _context.SaveChangesAsync();


                //// Obtiene el ID de la materia recién agregada
                //var materiaId = subject.MateriaId;

                //// Crea la relación entre Grupo y Materia
                //GruposMaterias gruposMaterias = new GruposMaterias
                //{
                //    GrupoId = 1, // Asigna el GrupoId que necesitas
                //    MateriaId = materiaId // Usa el ID obtenido
                //};

                //// Agrega la relación al contexto
                //_context.GruposMaterias.Add(gruposMaterias);
                //await _context.SaveChangesAsync(); // Guarda nuevamente

                // Devuelve la lista actualizada de materias
                return Ok(await _context.Materias.ToListAsync());
            }
            catch (DbUpdateException ex)
            {
                // Captura la excepción interna para más detalles
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Internal server error: {innerException}");
            }
        }





        //[HttpPost]
        //public async Task<ActionResult<List<MateriaRegistro>>> AddSubject([FromBody] MateriaRegistro subject)
        //{
        //    try
        //    {
        //        _context.Materias.Add(subject);
        //        await _context.SaveChangesAsync();

        //        GruposMaterias gruposMaterias = new GruposMaterias();

        //        gruposMaterias.GrupoId = 1;
        //        gruposMaterias.MateriaId = materiaId;

        //        _context.GruposMaterias.Add(gruposMaterias);
        //        await _context.SaveChangesAsync();
        //        return Ok(await _context.Materias.ToListAsync());
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // Captura la excepción interna para más detalles
        //        var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //        return StatusCode(500, $"Internal server error: {innerException}");
        //    }
        //}

        [HttpPut]
        public async Task<ActionResult<List<MateriaRegistro>>> UpdateSubject(MateriaRegistro updatedSubject)
        {
            var dbSubject = await _context.Materias.FindAsync(updatedSubject.MateriaId);
            if (dbSubject is null) return NotFound("Materia no encontrado");


            dbSubject.NombreMateria = updatedSubject.NombreMateria;
            dbSubject.Descripcion = updatedSubject.Descripcion;

            await _context.SaveChangesAsync();
            return Ok(await _context.Materias.ToListAsync());
        }

        [HttpDelete]
        public async Task<ActionResult<List<MateriaController>>> DeleteSubject(int id)
        {
            var dbSubject = await _context.Materias.FindAsync(id);
            if (dbSubject is null) return NotFound("Materia no encontrada");

            _context.Materias.Remove(dbSubject);
            await _context.SaveChangesAsync();
            return Ok(await _context.Materias.ToListAsync());
        }

    }
}
