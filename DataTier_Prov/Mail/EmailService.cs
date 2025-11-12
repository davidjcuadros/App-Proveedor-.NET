using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace DataTier_Prov.Mail;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_configuration["Email:FromName"], _configuration["Email:FromAddress"]));
        email.To.Add(new MailboxAddress("", to));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}