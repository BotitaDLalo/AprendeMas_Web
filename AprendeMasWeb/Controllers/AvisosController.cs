using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;

namespace AprendeMasWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvisosController(UserManager<IdentityUser> userManager, DataContext context) : ControllerBase
    {
        private readonly DataContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpPost("CrearAviso")]
        public async Task<ActionResult> CrearAviso([FromBody] PeticionCrearAviso crearAviso)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                Avisos avisos = new Avisos()
                {
                    DocenteId = crearAviso.DocenteId,
                    Titulo = crearAviso.Titulo,
                    Descripcion = crearAviso.Descripcion,
                    FechaCreacion = dateTime,
                };

                var materiaId = crearAviso.MateriaId;
                var grupoId = crearAviso.GrupoId;
                if (grupoId != null)
                {
                    avisos.GrupoId = grupoId;
                }
                else if (materiaId != null)
                {
                    avisos.MateriaId = materiaId;
                }

                await _context.tbAvisos.AddAsync(avisos);
                await _context.SaveChangesAsync();

                var nuevoAviso = await _context.tbAvisos.Where(a => a.AvisoId == avisos.AvisoId).FirstOrDefaultAsync();

                return Ok(nuevoAviso);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            finally
            {
                var message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = crearAviso.Titulo,
                        Body = crearAviso.Descripcion
                    },

                };
                List<int> lsAlumnosId = new List<int>();

                var materiaId = crearAviso.MateriaId;
                var grupoId = crearAviso.GrupoId;

                if (grupoId != null)
                {
                    lsAlumnosId = await _context.tbAlumnosGrupos.Where(a => a.GrupoId == grupoId).Select(a => a.AlumnoId).ToListAsync();
                }
                else if (materiaId != null)
                {
                    lsAlumnosId = await _context.tbAlumnosMaterias.Where(a => a.MateriaId == materiaId).Select(a => a.AlumnoId).ToListAsync();

                }

                foreach (var alumnoId in lsAlumnosId)
                {
                    var lsAlumnoTokens = await _context.tbAlumnosTokens.Where(a => a.AlumnoId == alumnoId).ToListAsync();

                    foreach (var alumnoToken in lsAlumnoTokens)
                    {
                        try
                        {
                            message.Token = alumnoToken.Token;
                            string result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                            Console.WriteLine(result);
                            break;
                        }
                        catch (FirebaseMessagingException fcme)
                        {
                            if (fcme.ErrorCode == ErrorCode.NotFound)
                            {
                                _context.tbAlumnosTokens.Remove(alumnoToken);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        [HttpGet("ConsultarAvisosCreados")]
        public async Task<ActionResult<List<RespuestaConsultarAvisos>>> ConsultarAvisos([FromBody] PeticionConsultarAvisos consultarAvisos)
        {
            try
            {
                List<RespuestaConsultarAvisos> lsResAvisos = new List<RespuestaConsultarAvisos>();
                List<Avisos> lsAvisos = new List<Avisos>();
                int grupoId = consultarAvisos.GrupoId;
                int materiaId = consultarAvisos.MateriaId;

                if (grupoId != 0)
                {
                    lsAvisos = await _context.tbAvisos.Where(a => a.GrupoId == grupoId).ToListAsync();


                }
                else if (materiaId != 0)
                {
                    lsAvisos = await _context.tbAvisos.Where(a => a.MateriaId == materiaId).ToListAsync();
                }

                foreach (var aviso in lsAvisos)
                {
                    int docenteId = aviso.DocenteId;
                    var docente = await _context.tbDocentes.Where(a => a.DocenteId == docenteId)
                        .Select(a => new
                        {
                            a.Nombre,
                            a.ApellidoPaterno,
                            a.ApellidoMaterno
                        }).FirstOrDefaultAsync();

                    RespuestaConsultarAvisos resAviso = new RespuestaConsultarAvisos()
                    {
                        AvisoId = aviso.AvisoId,
                        Titulo = aviso.Titulo,
                        Descripcion = aviso.Descripcion,
                        NombresDocente = docente?.Nombre ?? "",
                        ApePaternoDocente = docente?.ApellidoPaterno ?? "",
                        ApeMaternoDocente = docente?.ApellidoMaterno ?? "",
                        FechaCreacion = aviso.FechaCreacion,
                        GrupoId = aviso.GrupoId ?? 0,
                        MateriaId = aviso.MateriaId ?? 0
                    };

                    lsResAvisos.Add(resAviso);

                }
                return Ok(lsResAvisos.AsEnumerable().Reverse().ToList());

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        [HttpPost("EliminarAviso")]
        public async Task<ActionResult> EliminarAviso(int avisoId)
        {
            try
            {
                var aviso = await _context.tbAvisos.FindAsync(avisoId);

                if (aviso == null)
                {
                    return BadRequest();
                }

                _context.tbAvisos.Remove(aviso);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
