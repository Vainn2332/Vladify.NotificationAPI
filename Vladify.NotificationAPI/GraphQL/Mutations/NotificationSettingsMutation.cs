using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Models;

namespace Vladify.NotificationAPI.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class NotificationSettingsMutation(INotificationService _notificationService)
{
    public Task<UserNotificationSettingsModel> UpdateNotificationSettingsAsync(UserNotificationSettingsModel input, CancellationToken cancellationToken)
    {
        return _notificationService.UpdateAsync(input, cancellationToken);
    }

    public Task PatchSubscriptionAsync(UserNotificationSubscriptionPatchRequestModel input, CancellationToken cancellationToken)
    {
        return _notificationService.PatchSubscriptionAsync(input, cancellationToken);
    }
}
