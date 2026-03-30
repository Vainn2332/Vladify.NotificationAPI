using MongoDB.Driver;
using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess;

public class NotificationRepository(IMongoCollection<NotificationInfo> _notifications) : INotificationRepository
{
    public async Task<NotificationInfo> CreateAsync(NotificationInfo notification, CancellationToken cancellationToken)
    {
        await _notifications.InsertOneAsync(notification, new InsertOneOptions(), cancellationToken);

        return notification;
    }

    public async Task<List<NotificationInfo>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var items = await _notifications.Find(item => true)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<NotificationInfo?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _notifications.Find(item => item.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(NotificationInfo notification, CancellationToken cancellationToken)
    {
        await _notifications.ReplaceOneAsync(item => item.Id == notification.Id, notification, new ReplaceOptions(), cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _notifications.DeleteOneAsync(item => item.Id == id, cancellationToken);
    }
}
