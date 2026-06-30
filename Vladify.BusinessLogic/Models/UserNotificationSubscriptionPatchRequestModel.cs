namespace Vladify.BusinessLogic.Models;

public class UserNotificationSubscriptionPatchRequestModel
{
    public required string Id { get; set; }

    public bool? IsEmailSubscribed { get; set; }
}
