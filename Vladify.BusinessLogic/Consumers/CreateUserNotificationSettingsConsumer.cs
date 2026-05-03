using AutoMapper;
using MassTransit;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Messages;
using Vladify.BusinessLogic.Models;

namespace Vladify.BusinessLogic.Consumers;

public class CreateUserNotificationSettingsConsumer : IConsumer<UserCreatedMessage>
{
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public CreateUserNotificationSettingsConsumer(INotificationService notificationService, IMapper mapper)
    {
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public Task Consume(ConsumeContext<UserCreatedMessage> context)
    {
        var message = context.Message;

        var userNotificationSettings = _mapper.Map<UserNotificationSettingsRequestModel>(message);
        userNotificationSettings.NotificationSubscription.IsEmailSubscribed = true;

        return _notificationService.CreateAsync(userNotificationSettings, context.CancellationToken);
    }
}
