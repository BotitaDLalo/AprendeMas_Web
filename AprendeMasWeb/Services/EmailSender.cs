using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit.Text;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace AprendeMasWeb.Services
{
    public class EmailSender(IConfiguration configuration) : IEmailSender
    {
        private readonly IConfiguration _configuration = configuration;

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var fromConf = _configuration["EmailConfiguration:From"];
                var serverConf = _configuration["EmailConfiguration:SMPTServer"];
                var portConf = _configuration["EmailConfiguration:Port"];
                var passwordConf = _configuration["EmailConfiguration:Password"];
                if (serverConf == null || portConf == null)
                {
                    throw new Exception("Hubo un error en el envio de correo");
                }

                var emailGenerado = new MimeMessage();
                emailGenerado.From.Add(MailboxAddress.Parse(fromConf));
                emailGenerado.To.Add(MailboxAddress.Parse(email));
                emailGenerado.Subject = subject; //TODO: Preparar el titulo del correo
                emailGenerado.Body = new TextPart(TextFormat.Html) { Text = htmlMessage }; //TODO: poner codigo para cambio de password


                var smtp = new SmtpClient();
                smtp.Connect(serverConf, Int32.Parse(portConf), SecureSocketOptions.StartTls);
                smtp.Authenticate(fromConf, passwordConf);
                smtp.Send(emailGenerado);
                smtp.Disconnect(true);


                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw new Exception("Hubo un error en el envio de correo");
            }
        }
    }
}
