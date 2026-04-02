
using Vladify.BuisnessLogic.Models;

namespace Vladify.BuisnessLogic.Interfaces;

public interface INotificationService
{
    public Task<NotificationModel> CreateAsync(NotificationModel notificationModel, CancellationToken cancellationToken);
    public Task<NotificationModel?> GetByIdAsync(string id, CancellationToken cancellationToken);
    public Task<IEnumerable<NotificationModel>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<NotificationModel> UpdateAsync(NotificationModel notificationModel, CancellationToken cancellationToken);
    public Task DeleteAsync(string id, CancellationToken cancellationToken);
}
