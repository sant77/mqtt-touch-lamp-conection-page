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
            var address = Environment.GetEnvironmentVariable("ADRESS");

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
}
}