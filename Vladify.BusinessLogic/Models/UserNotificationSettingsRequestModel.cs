namespace Vladify.BusinessLogic.Models;

public class UserNotificationSettingsRequestModel
{
    public required Guid UserId { get; set; }

    public required string EmailAddress { get; set; }

    public required NotificationSubscriptionModel NotificationSubscription { get; set; }
}
