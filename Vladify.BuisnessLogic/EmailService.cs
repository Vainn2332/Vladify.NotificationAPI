using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Vladify.BuisnessLogic.Constants;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Models;
using Vladify.BuisnessLogic.Options;

namespace Vladify.BuisnessLogic;

public class EmailService : IEmailService
{
    private readonly EmailNotificationOptions _options;
    private readonly INotificationService _notificationService;
    private readonly ILogger<EmailService> _logger;
    private readonly ISmtpClientFactory _factory;

    public EmailService(IOptions<EmailNotificationOptions> options, INotificationService notificationService, ILogger<EmailService> logger, ISmtpClientFactory factory)
    {
        _options = options.Value;
        _notificationService = notificationService;
        _logger = logger;
        _factory = factory;
    }

    public async Task SendToAllUsersAsync(string subject, string message, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = BusinessLogicConstants.MaxAmountOfParallelThreadsForEmailNotification,
            CancellationToken = cancellationToken
        };
        IEnumerable<UserNotificationSettingsModel> subscribers;
        int pageNumber = 1;

        do
        {
            subscribers = await _notificationService.GetEmailSubscribersAsync(pageNumber++, BusinessLogicConstants.NotificationBatchSize, cancellationToken);
            var chunks = subscribers.Chunk(BusinessLogicConstants.ChunkSize);

            await Parallel.ForEachAsync(chunks, parallelOptions, async (chunk, cancellationToken) =>
            {
                await ProcessNotificationChunkAsync(chunk, subject, message, cancellationToken);
            });
        }
        while (subscribers.Any());
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

    private async Task ProcessNotificationChunkAsync(IEnumerable<UserNotificationSettingsModel> chunk, string subject, string message, CancellationToken ct)
    {
        try
        {
            using var client = await _factory.CreateClientAsync(ct);
            foreach (var notificationInfo in chunk)
            {
                var mail = CreateMessage(notificationInfo.EmailAddress, subject, message);
                await client.SendAsync(mail, ct);
            }
            await client.DisconnectAsync(true, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error happened while trying to notify user via email");
        }
    }
}
