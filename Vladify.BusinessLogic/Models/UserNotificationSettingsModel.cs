namespace Vladify.BusinessLogic.Models;

public class UserNotificationSettingsModel
{
    public required string Id { get; set; }

    public required Guid UserId { get; set; }

    public required string EmailAddress { get; set; }

    public required NotificationSubscriptionModel NotificationSubscription { get; set; }
}
