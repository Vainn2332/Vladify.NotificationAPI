using MassTransit;
using Vladify.BusinessLogic.Constants;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Messages;

namespace Vladify.BusinessLogic.Consumers;

public class EmailSenderConsumer : IConsumer<SongCreatedMessage>
{
    private readonly IEmailService _emailService;

    public EmailSenderConsumer(IEmailService emailService)
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
