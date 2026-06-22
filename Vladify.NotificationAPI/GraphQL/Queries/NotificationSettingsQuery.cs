using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Models;

namespace Vladify.NotificationAPI.GraphQL.Queries;

[ExtendObjectType("Query")]
public class NotificationSettingsQuery(INotificationService _notificationService)
{
    public Task<UserNotificationSettingsModel?> GetNotificationByIdAsync(string id, CancellationToken cancellationToken)
    {
        return _notificationService.GetByIdAsync(id, cancellationToken);
    }

    public Task<List<UserNotificationSettingsModel>> GetNotificationsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return _notificationService.GetAllAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<List<UserNotificationSettingsModel>> GetEmailSubscribersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return _notificationService.GetEmailSubscribersAsync(pageNumber, pageSize, cancellationToken);
    }
}
