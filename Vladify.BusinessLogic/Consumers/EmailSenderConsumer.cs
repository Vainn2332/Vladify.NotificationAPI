using DnsClient.Internal;
using MassTransit;
using Microsoft.Extensions.Logging;
using Vladify.BusinessLogic.Constants;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Messages;

namespace Vladify.BusinessLogic.Consumers;

public class EmailSenderConsumer : IConsumer<SongCreatedMessage>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailSenderConsumer> _logger;

    public EmailSenderConsumer(IEmailService emailService, ILogger<EmailSenderConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SongCreatedMessage> context)
    {
        _logger.LogInformation("SongCreatedMessage received");

        var receivedMessage = context.Message;
        var message = string.Format(BusinessLogicConstants.SongCreatedMessageTemplate, receivedMessage.Author, receivedMessage.Title, receivedMessage.Album);

        await _emailService.SendToAllUsersAsync(BusinessLogicConstants.SongCreatedEmailSubject, message, context.CancellationToken);

        _logger.LogInformation("Emails sent");
    }
}
