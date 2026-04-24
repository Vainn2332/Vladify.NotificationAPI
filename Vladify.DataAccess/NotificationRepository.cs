using MongoDB.Driver;
using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess;

public class NotificationRepository(IMongoCollection<UserNotificationSettings> _notifications) : INotificationRepository
{
    public async Task<UserNotificationSettings> CreateAsync(UserNotificationSettings notification, CancellationToken cancellationToken)
    {
        await _notifications.InsertOneAsync(notification, new InsertOneOptions(), cancellationToken);

        return notification;
    }

    public async Task<IEnumerable<UserNotificationSettings>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _notifications.Find(item => true)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserNotificationSettings?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _notifications.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserNotificationSettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _notifications.Find(item => item.UserId == userId).FirstOrDefaultAsync(cancellationToken);
    }

    public Task UpdateAsync(UserNotificationSettings notification, CancellationToken cancellationToken)
    {
        return _notifications.ReplaceOneAsync(item => item.Id == notification.Id, notification, new ReplaceOptions() { IsUpsert = true }, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        return _notifications.DeleteOneAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserNotificationSettings>> GetEmailSubscribersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _notifications.Find(item => item.NotificationSubscription.IsEmailSubscribed)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }
}
