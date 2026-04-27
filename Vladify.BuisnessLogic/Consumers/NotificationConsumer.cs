using MassTransit;
using Vladify.BuisnessLogic.Constants;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Messages;

namespace Vladify.BuisnessLogic.Consumers;

public class NotificationConsumer : IConsumer<SongCreatedMessage>
{
    private readonly IEmailService _emailService;

    public NotificationConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task Consume(ConsumeContext<SongCreatedMessage> context)
    {
        var receivedMessage = context.Message;
        var message = string.Format(BusinessLogicConstants.SongCreatedMessageTemplate, receivedMessage.Author, receivedMessage.Title, receivedMessage.Album);

        return _emailService.SendToAllUsersAsync(BusinessLogicConstants.SongCreatedEmailSubject, message, context.CancellationToken);
    }
}
