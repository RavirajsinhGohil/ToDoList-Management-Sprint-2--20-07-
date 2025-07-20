using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        MimeMessage? emailToSend = new();
        emailToSend.From.Add(MailboxAddress.Parse(_configuration["MailSettings:Mail"]));
        emailToSend.To.Add(MailboxAddress.Parse(email));
        emailToSend.Subject = subject;
        emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

        string portValue = _configuration["MailSettings:Port"] ?? throw new ArgumentNullException("MailSettings:Port configuration is missing.");
        using (SmtpClient? emailClient = new())
        {
            emailClient.Connect(_configuration["MailSettings:Host"], int.Parse(portValue), SecureSocketOptions.StartTls);
            emailClient.Authenticate(_configuration["MailSettings:Mail"], _configuration["MailSettings:Password"]);
            emailClient.Send(emailToSend);
            emailClient.Disconnect(true);
        }
        await Task.CompletedTask;
    }
}