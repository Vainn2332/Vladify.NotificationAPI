using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess;

public interface INotificationRepository
{
    public Task<NotificationInfo> CreateAsync(NotificationInfo notification, CancellationToken cancellationToken);
    public Task<NotificationInfo?> GetByIdAsync(string id, CancellationToken cancellationToken);
    public Task<IEnumerable<NotificationInfo>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<NotificationInfo?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    public Task UpdateAsync(NotificationInfo notification, CancellationToken cancellationToken);
    public Task DeleteAsync(string id, CancellationToken cancellationToken);
}
