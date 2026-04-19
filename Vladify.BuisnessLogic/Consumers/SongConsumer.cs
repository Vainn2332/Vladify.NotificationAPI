using MassTransit;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Models.Messages;

namespace Vladify.BuisnessLogic.Consumers;

public class SongConsumer : IConsumer<SongMessage>
{
    private readonly IEmailService _emailService;

    public SongConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }


    public Task Consume(ConsumeContext<SongMessage> context)
    {
        var songData = context.Message;
        var message =
            @$"<p>У {songData.Author} в альбоме {songData.Album} вышел новый трек {songData.Title}! Успей заценить</p>";
        var subject = "Новый трек";

        return _emailService.SendToAllUsersAsync(subject, message, context.CancellationToken);
    }
}
