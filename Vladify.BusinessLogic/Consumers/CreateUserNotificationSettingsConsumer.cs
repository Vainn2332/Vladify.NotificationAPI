using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Messages;
using Vladify.BusinessLogic.Models;

namespace Vladify.BusinessLogic.Consumers;

public class CreateUserNotificationSettingsConsumer : IConsumer<UserCreatedMessage>
{
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserNotificationSettingsConsumer> _logger;

    public CreateUserNotificationSettingsConsumer(INotificationService notificationService, IMapper mapper, ILogger<CreateUserNotificationSettingsConsumer> logger)
    {
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedMessage> context)
    {
        _logger.LogInformation("UserCreatedMessage received");
        var message = context.Message;

        var userNotificationSettings = _mapper.Map<UserNotificationSettingsRequestModel>(message);
        userNotificationSettings.NotificationSubscription.IsEmailSubscribed = true;

        await _notificationService.CreateAsync(userNotificationSettings, context.CancellationToken);

        _logger.LogInformation("User created");
    }
}
