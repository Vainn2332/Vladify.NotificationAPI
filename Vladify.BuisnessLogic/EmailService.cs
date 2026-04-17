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
        IEnumerable<NotificationModel> notificationsPart;
        int pageNumber = 1;

        do
        {
            notificationsPart = await _notificationService.GetAllAsync(pageNumber++, BusinessLogicConstants.NotificationBatchSize, cancellationToken);
            var chunks = notificationsPart.Chunk(BusinessLogicConstants.ChunkSize);

            await Parallel.ForEachAsync(chunks, parallelOptions, async (chunk, cancellationToken) =>
            {
                try
                {
                    using var client = await _factory.CreateClientAsync(cancellationToken);

                    var mail = CreateMessage(chunk, subject, message);
                    await client.SendAsync(mail, cancellationToken);

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

    private MimeMessage CreateMessage(NotificationModel[] recepientChunk, string subject, string message)
    {
        var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        foreach (var notificationInfo in recepientChunk)
        {
            var isSubcrbibedToEmailNotifications = notificationInfo.NotificationSubscription.Email;
            if (!isSubcrbibedToEmailNotifications)
            {
                continue;
            }
            mail.To.Add(MailboxAddress.Parse(notificationInfo.EmailAddress));
        }
        mail.Subject = subject;
        mail.Body = new TextPart(TextFormat.Html) { Text = message };

        return mail;
    }
}
