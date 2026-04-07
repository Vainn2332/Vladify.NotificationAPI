namespace Vladify.BuisnessLogic.Models;

public class NotificationRequestModel
{
    public required Guid UserId { get; set; }

    public required string EmailAddress { get; set; }

    public required string FirebaseCloudMessagingToken { get; set; }

    public required NotificationSubscriptionModel NotificationSubscription { get; set; }
}
