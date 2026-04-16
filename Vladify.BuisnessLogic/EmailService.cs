using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Net;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Models;
using Vladify.BuisnessLogic.Options;

namespace Vladify.BuisnessLogic;
public class EmailService : IEmailService
{
    private readonly EmailNotificationOptions _options;
    private readonly INotificationService _notificationService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailNotificationOptions> options, INotificationService notificationService, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task SendToAllUsersAsync(string subject, string message, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 5,
            CancellationToken = cancellationToken
        };
        IEnumerable<NotificationModel> notificationsPart;
        int pageNumber = 1;

        do
        {
            notificationsPart = await _notificationService.GetAllAsync(pageNumber++, 100, cancellationToken);
            var chunks = notificationsPart.Chunk(20);

            await Parallel.ForEachAsync(chunks, parallelOptions, async (chunk, cancellationToken) =>
            {
                try
                {
                    using var client = await CreateSmtpClientAsync(cancellationToken);

                    foreach (var notificationInfo in chunk)
                    {
                        var isSubcrbibedToEmailNotifications = notificationInfo.NotificationSubscription.Email;
                        if (!isSubcrbibedToEmailNotifications)
                        {
                            continue;
                        }

                        var mail = CreateMessage(notificationInfo.EmailAddress, subject, message);
                        await client.SendAsync(mail, cancellationToken);
                    }
                    await client.DisconnectAsync(true, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error happened while trying to notify user via email");
                }
            });
        }
        while (notificationsPart.Any());
    }

    private MimeMessage CreateMessage(string recepientEmail, string subject, string message)
    {
        var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        mail.To.Add(MailboxAddress.Parse(recepientEmail));
        mail.Subject = subject;
        mail.Body = new TextPart(TextFormat.Html) { Text = message };

        return mail;
    }

    private async Task<SmtpClient> CreateSmtpClientAsync(CancellationToken cancellationToken)
    {
        var client = new SmtpClient();
        await client.ConnectAsync(_options.SMTPClientUrl, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
        var creds = new NetworkCredential(_options.SenderEmail, _options.ApplicationPassword);
        await client.AuthenticateAsync(creds, cancellationToken);

        return client;
    }
}
