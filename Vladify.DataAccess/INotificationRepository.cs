using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess;

public interface INotificationRepository
{
    public Task<UserNotificationSettings> CreateAsync(UserNotificationSettings notification, CancellationToken cancellationToken);
    public Task<UserNotificationSettings?> GetByIdAsync(string id, CancellationToken cancellationToken);
    public Task<IEnumerable<UserNotificationSettings>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<IEnumerable<UserNotificationSettings>> GetEmailSubscribersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<UserNotificationSettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    public Task UpdateAsync(UserNotificationSettings notification, CancellationToken cancellationToken);
    public Task DeleteAsync(string id, CancellationToken cancellationToken);
}
