using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace userService.Services{ 

public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = Environment.GetEnvironmentVariable("EMAIL");
    private readonly string _smtpPass = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

    public async Task SendConfirmationEmail(string email, string token)
    {
        try
        {
            var address = Environment.GetEnvironmentVariable("ADRESS_SERVER");

            string confirmationLink = $"http://{address}/userservice/v1/user/confirm?token={token}";
            string htmlBody = $@"
                <html>
                <body>
                    <h2>Confirmación de Correo</h2>
                    <p>Gracias por registrarte. Para confirmar tu correo, haz clic en el siguiente enlace:</p>
                    <a href='{confirmationLink}' style='padding:10px;background:#28a745;color:white;text-decoration:none;'>Confirmar Correo</a>
                    <p>Si no solicitaste este registro, ignora este mensaje.</p>
                </body>
                </html>";

            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser, "Tu Empresa"),
                    Subject = "Confirma tu correo electrónico",
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar correo: {ex.Message}");
        }
    }

    public async Task SendPasswordResetEmail(string email, string resetToken)
    {
        try
        {
            var address = Environment.GetEnvironmentVariable("ADRESS_SERVER");
            
            string resetLink = $"http://{address}/reset-password?email={WebUtility.UrlEncode(email)}";

            string htmlBody = $@"
            <html>
            <body>
                <h2>Recuperación de Contraseña</h2>
                <p>Hemos recibido una solicitud para restablecer tu contraseña. Utiliza el siguiente PIN:</p>
                <div style='font-size: 24px; font-weight: bold; margin: 20px 0;'>{resetToken}</div>
                <p>Este PIN expirará en 15 minutos.</p>
                <p>O haz clic en el siguiente enlace para ir directamente a la página de recuperación:</p>
                <a href='{resetLink}' style='padding:10px;background:#ff8906;color:white;text-decoration:none;border-radius:5px;'>
                    Restablecer contraseña
                </a>
                <p style='margin-top:20px;font-size:12px;color:#a7a9be;'>
                    Si no solicitaste este cambio, ignora este mensaje.
                </p>
            </body>
            </html>";

           using (var client = new SmtpClient(_smtpServer, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser, "Tu Empresa"),
                Subject = "Recuperación de contraseña",
                Body = htmlBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar correo de recuperación: {ex.Message}");
            throw;
        }
    }

}
}