using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using AprendeMasWeb.Models.RestablecerPassword;
namespace AprendeMasWeb.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CorreoRestablecerPasswordController(IConfiguration configuration, IEmailSender emailSender, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpPost("EnvioCodigo")]
        public async Task<IActionResult> EnvioCodigoRestablecer([FromBody] EnvioCodigoRestablecer envioCodigoRestablecer)
        {
            try
            {
                //TODO: logica para generar token de restablecer contra
                var usuario = await _userManager.FindByEmailAsync(envioCodigoRestablecer.Destinatario);
                if (usuario == null)
                {
                    return BadRequest(new { mensaje = "El usuario no existe" });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

                //var callback = Url.Action("EnvioCodigoRestablecer", "CorreoRestablecerPassword", new { token, email = usuario.Email }, Request.Scheme);
                //var callback = Url.Action("InicioSesionUsuario", "Login", new { token, email = usuario.Email }, Request.Scheme);

                var callback = Url.Action("Index", "RestablecerPassword", new { token, email = usuario.Email }, Request.Scheme);

                await _emailSender.SendEmailAsync(envioCodigoRestablecer.Destinatario, "Restablecer contraseña", callback ?? throw new ArgumentNullException(callback, "No se creo el link para restablecer contraseña"));

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "La correo no se envio" });
            }
        }
    }
}
