using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DataTier_Prov.Mail
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                _logger.LogInformation("📨 Preparando correo para enviar a {To}", to);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_configuration["Email:FromName"], _configuration["Email:FromAddress"]));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                _logger.LogInformation("🔌 Conectando al servidor SMTP {Server}:{Port}", 
                    _configuration["Email:SmtpServer"], _configuration["Email:SmtpPort"]);

                await smtp.ConnectAsync(
                    _configuration["Email:SmtpServer"], 
                    int.Parse(_configuration["Email:SmtpPort"]), 
                    SecureSocketOptions.StartTls);

                _logger.LogInformation("🔐 Autenticando con el servidor SMTP...");
                await smtp.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("✅ Correo enviado correctamente a {To} con asunto '{Subject}'", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al enviar correo a {To}", to);
                throw;
            }
        }
    }
}
