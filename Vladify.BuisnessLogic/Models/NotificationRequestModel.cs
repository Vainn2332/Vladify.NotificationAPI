using Vladify.DataAccess.Entities;

namespace Vladify.BuisnessLogic.Models;

public class NotificationRequestModel
{
    public required string UserId { get; set; }

    public required string EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public required NotificationSubscription NotificationSubscription { get; set; }
}
