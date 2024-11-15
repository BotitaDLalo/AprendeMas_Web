﻿using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriasController : Controller
    {
        private readonly DataContext _context;

        public MateriasController(DataContext context)
        {
            _context = context;
        }


        public async Task<List<object>> ConsultaGrupos()
        {
            try
            {
                var lsGrupos = await _context.tbGrupos.ToListAsync();


                var listaGruposMaterias = new List<object>();
                foreach (var grupo in lsGrupos)
                {
                    var lsMateriasId = await _context.tbGruposMaterias.Where(a => a.GrupoId == grupo.GrupoId).Select(a => a.MateriaId).ToListAsync();

                    var lsMaterias = await _context.tbMaterias.Where(a => lsMateriasId.Contains(a.MateriaId)).Select(m => new
                    {
                        m.MateriaId,
                        m.NombreMateria,
                        m.Descripcion,
                        //m.CodigoColor,
                        actividades = _context.tbActividades.Where(a => a.MateriaId == m.MateriaId).ToList()
                    }).ToListAsync();


                    listaGruposMaterias.Add(new
                    {
                        grupoId = grupo.GrupoId,
                        nombreGrupo = grupo.NombreGrupo,
                        descripcion = grupo.Descripcion,
                        codigoAcceso = grupo.CodigoAcceso,
                        codigoColor = grupo.CodigoColor,
                        materias = lsMaterias
                    });
                }

                return listaGruposMaterias;
            }
            catch (Exception)
            {
                return [];
            }
        }


        public async Task<List<MateriaRegistro>> ConsultaMaterias()
        {
            try
            {
                var lsGruposMaterias = await _context.tbGruposMaterias.Select(a => a.MateriaId).ToListAsync();

                var lsMateriasSinGrupo = await _context.tbMaterias.Where(a => !lsGruposMaterias.Contains(a.MateriaId)).ToListAsync();

                return lsMateriasSinGrupo;
            }
            catch (Exception)
            {
                return [];
            }
        }

        [HttpGet("ObtenerMaterias")]
        public async Task<ActionResult<List<MateriaRegistro>>> ObtenerMaterias()
        {
            try
            {
                return await ConsultaMaterias();
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    mensaje = "Hubo un error en ObtenerMaterias"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaRegistro>> ObtenerMateriaUnica(int id)
        {
            var subject = await _context.tbMaterias.FindAsync(id);
            if (subject is null) return NotFound("Materia no encontrado");

            return Ok(subject);
        }

        [HttpPost("MateriaSinGrupo")]
        public async Task<ActionResult> CrearMateriaSinGrupo([FromBody] MateriaRegistro materia)
        {
            try
            {
                _context.tbMaterias.Add(materia);
                await _context.SaveChangesAsync();
                return Ok(await ConsultaMaterias());
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "No se registro la materia" });
            }
        }

        [HttpPost("MateriaGrupos")]
        public async Task<ActionResult<List<MateriaConGrupo>>> CrearMateriaGrupos([FromBody] MateriaConGrupo materiaConGrupo)
        {
            try
            {
                MateriaRegistro materia = new()
                {
                    NombreMateria = materiaConGrupo.NombreMateria,
                    Descripcion = materiaConGrupo.Descripcion,
                    //CodigoColor = materiaG.CodigoColor,
                };


                _context.tbMaterias.Add(materia);
                await _context.SaveChangesAsync();

                var idMateria = materia.MateriaId;
                List<int> gruposVinculados = materiaConGrupo.Grupos;

                foreach (var grupo in gruposVinculados)
                {
                    GruposMaterias gruposMaterias = new()
                    {
                        GrupoId = grupo,
                        MateriaId = idMateria
                    };

                    _context.tbGruposMaterias.Add(gruposMaterias);
                }
                await _context.SaveChangesAsync();

                var lsGruposMaterias = await ConsultaGrupos();

                return Ok(lsGruposMaterias);
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
            var dbSubject = await _context.tbMaterias.FindAsync(updatedSubject.MateriaId);
            if (dbSubject is null) return NotFound("Materia no encontrado");


            dbSubject.NombreMateria = updatedSubject.NombreMateria;
            dbSubject.Descripcion = updatedSubject.Descripcion;

            await _context.SaveChangesAsync();
            return Ok(await _context.tbMaterias.ToListAsync());
        }

        [HttpDelete]
        public async Task<ActionResult<List<MateriasController>>> DeleteSubject(int id)
        {
            var dbSubject = await _context.tbMaterias.FindAsync(id);
            if (dbSubject is null) return NotFound("Materia no encontrada");

            _context.tbMaterias.Remove(dbSubject);
            await _context.SaveChangesAsync();
            return Ok(await _context.tbMaterias.ToListAsync());
        }

    }
}
