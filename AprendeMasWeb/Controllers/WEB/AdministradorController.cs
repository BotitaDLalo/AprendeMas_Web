using AprendeMasWeb.Data;
using AprendeMasWeb.Models;
using AprendeMasWeb.Models.DBModels;
using AprendeMasWeb.Recursos;
using AprendeMasWeb.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace AprendeMasWeb.Controllers.WEB
{
    public class AdministradorController(IEmailSender emailSender, UserManager<IdentityUser> userManager, DataContext context) : Controller
    {
        private readonly DataContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager; private readonly IEmailSender _emailSender = emailSender;

        public async Task<IActionResult> Index()
        {
            List<DocentesValidacion> lsDocentesAdministrar = new List<DocentesValidacion>();
            var lsDocentes = _context.tbDocentes.ToList();

            foreach (var d in lsDocentes)
            {
                string email = await ObtenerCorreoDocente(d.DocenteId);
                var autorizado = EstadoAutorizado(d.estaAutorizado);
                var envioCorreo = EnvioCorreo(d.seEnvioCorreo ?? false);

                DocentesValidacion docente = new()
                {
                    DocenteId = d.DocenteId,
                    ApellidoPaterno = d.ApellidoPaterno,
                    ApellidoMaterno = d.ApellidoMaterno,
                    Nombre = d.Nombre,
                    Email = email,
                    Autorizado = autorizado,
                    EnvioCorreo = envioCorreo
                };
                lsDocentesAdministrar.Add(docente);
            }

            return View(lsDocentesAdministrar);
        }
        private static string EstadoAutorizado(bool? status)
        {

            if (status == null)
            {
                return EstatusAutorizacion.PENDIENTE;
            }
            else
            {
                if (status.Value)
                {
                    return EstatusAutorizacion.AUTORIZADO;
                }
                else
                {
                    return EstatusAutorizacion.DENEGADO;
                }
            }

        }

        private static string EnvioCorreo(bool status)
        {
            if (status)
            {
                return EstatusEnvioCorreoDocente.ENVIADO;
            }
            else
            {
                return EstatusEnvioCorreoDocente.NO_ENVIADO;
            }
        }

        private async Task<string> ObtenerCorreoDocente(int docenteId)
        {
            var docenteUserId = _context.tbDocentes.Where(a => a.DocenteId == docenteId).Select(a => a.UserId).FirstOrDefault();

            var user = await _userManager.FindByIdAsync(docenteUserId ?? "");

            if (user != null)
            {
                var email = user.Email;
                return email ?? "";
            }
            return "";
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutorizarDocente([FromBody] int docenteId)
        {
            try
            {
                var docente = await _context.tbDocentes.Where(a => a.DocenteId == docenteId).FirstOrDefaultAsync();

                if (docente != null)
                {
                    var userId = docente.UserId;

                    var codigoDocente = docente.CodigoAutorizacion;

                    var user = await _userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        try
                        {
                            var email = user.Email;

                            await _emailSender.SendEmailAsync(email ?? "", "Código de verificacion", codigoDocente ?? "");

                            docente.seEnvioCorreo = true;
                            _context.SaveChanges();
                            return Ok(new { mensaje = "Código de verificación enviado con exito." });
                        }
                        catch (Exception)
                        {
                            return BadRequest(new { mensaje = "No se pudo mandar código de verificación." });
                        }
                    }
                }
                return BadRequest();

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DenegarDocente([FromBody] int docenteId)
        {
            try
            {
                var docente = await _context.tbDocentes.Where(a => a.DocenteId == docenteId).FirstOrDefaultAsync();

                if (docente != null)
                {
                    docente.estaAutorizado = false;
                    docente.seEnvioCorreo = false;
                    docente.CodigoAutorizacion = null;
                    docente.CodigoAutorizacion = null;
                    docente.FechaExpiracionCodigo = null;

                    _context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReenviarCodigo([FromBody] int docenteId)
        {
            try
            {
                var docente = await _context.tbDocentes.Where(a => a.DocenteId == docenteId).FirstOrDefaultAsync();

                if (docente != null)
                {
                    var userId = docente.UserId;

                    var codigoDocente = docente.CodigoAutorizacion;
                    var fechaLimite = docente.FechaExpiracionCodigo;

                    if (fechaLimite < DateTime.Now)
                    {
                        
                        bool existeCodigo = false;

                        do
                        {
                            existeCodigo = _context.tbDocentes.Any(a => a.CodigoAutorizacion == codigoDocente);

                            if (existeCodigo)
                            {
                                codigoDocente = RecursosGenerales.GenerarCodigoAleatorio();
                            }
                        }
                        while (existeCodigo);
                        DateTime fechaExpiracionCodigo = DateTime.UtcNow.AddMinutes(59);
                        docente.FechaExpiracionCodigo = fechaExpiracionCodigo;
                        docente.CodigoAutorizacion = codigoDocente;

                        _context.SaveChanges();
                    }

                    var user = await _userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        try
                        {
                            var email = user.Email;

                            await _emailSender.SendEmailAsync(email ?? "", "Código de verificacion", codigoDocente ?? "");

                            docente.seEnvioCorreo = true;
                            _context.SaveChanges();
                            return Ok(new { mensaje = "Código de verificación enviado con exito." });
                        }
                        catch (Exception)
                        {
                            return BadRequest(new { mensaje = "No se pudo mandar código de verificación." });
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                return BadRequest();

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
