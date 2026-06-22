using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Models;
using Vladify.NotificationAPI.Constants;

namespace Vladify.NotificationAPI.GraphQL.Mutations;

[ExtendObjectType(GraphQlConstants.MutationName)]
public class NotificationSettingsMutation(INotificationService _notificationService)
{
    public Task<UserNotificationSettingsModel> UpdateNotificationSettingsAsync(UserNotificationSettingsModel input, CancellationToken cancellationToken)
    {
        return _notificationService.UpdateAsync(input, cancellationToken);
    }

    public async Task<bool> PatchSubscriptionAsync(UserNotificationSubscriptionPatchRequestModel input, CancellationToken cancellationToken)
    {
        await _notificationService.PatchSubscriptionAsync(input, cancellationToken);

        return true;
    }
}
