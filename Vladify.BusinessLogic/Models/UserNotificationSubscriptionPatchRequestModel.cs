namespace Vladify.BusinessLogic.Models;

public class UserNotificationSubscriptionPatchRequestModel
{
    public required string Id { get; set; }

    // there will be other properties related to other notification subscriptions in the future
    public bool? IsEmailSubscribed { get; set; }
}
