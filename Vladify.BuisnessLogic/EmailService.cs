using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Net;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Options;

namespace Vladify.BuisnessLogic;
public class EmailService : IEmailService
{
    private readonly EmailNotificationOptions _options;

    public EmailService(IOptions<EmailNotificationOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string receiver, string subject, string message, CancellationToken cancellationToken)
    {
        var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        mail.To.Add(MailboxAddress.Parse(receiver));
        mail.Subject = subject;
        mail.Body = new TextPart(TextFormat.Html) { Text = message };

        using var client = new SmtpClient();

        await client.ConnectAsync(_options.SMTPClientUrl, _options.Port, SecureSocketOptions.StartTls, cancellationToken);

        var creds = new NetworkCredential(_options.SenderEmail, _options.ApplicationPassword);
        await client.AuthenticateAsync(creds, cancellationToken);

        await client.SendAsync(mail);
    }
}
