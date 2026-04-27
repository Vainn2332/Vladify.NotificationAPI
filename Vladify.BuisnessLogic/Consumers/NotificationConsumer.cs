using MassTransit;
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
        var message =
            @$"<p>{receivedMessage.Author} has posted {receivedMessage.Title} in his album {receivedMessage.Album}!</p>
            <p>Don't forget to check it up</p>";

        return _emailService.SendToAllUsersAsync("Новая песня", message, context.CancellationToken);
    }
}
