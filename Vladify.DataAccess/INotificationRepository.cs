using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess;

public interface INotificationRepository
{
    public Task<NotificationInfo> CreateAsync(NotificationInfo notification);
    public Task<NotificationInfo?> GetByIdAsync(string id);
    public Task<List<NotificationInfo>> GetAllAsync(string pageNumber, string pageSize);
    public Task UpdateAsync(NotificationInfo notification);
    public Task DeleteAsync(string id);
}
