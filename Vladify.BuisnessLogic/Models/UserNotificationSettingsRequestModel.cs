namespace Vladify.BuisnessLogic.Models;

public class UserNotificationSettingsRequestModel
{
    public required Guid UserId { get; set; }

    public required string EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public required NotificationSubscriptionModel NotificationSubscription { get; set; }
}
