using MongoDB.Driver;
using Vladify.DataAccess.Dto;
using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess;

public class NotificationRepository(IMongoCollection<UserNotificationSettings> _notifications) : INotificationRepository
{
    public async Task<UserNotificationSettings> CreateAsync(UserNotificationSettings notification, CancellationToken cancellationToken)
    {
        await _notifications.InsertOneAsync(notification, new InsertOneOptions(), cancellationToken);

        return notification;
    }

    public Task<List<UserNotificationSettings>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return _notifications.Find(item => true)
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

    public Task<List<UserNotificationSettings>> GetEmailSubscribersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return _notifications.Find(item => item.NotificationSubscription.IsEmailSubscribed)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task PatchSubscriptionAsync(PatchSubscriptionDto patchSubscriptionDto, CancellationToken cancellationToken)
    {
        var updateBuilder = Builders<UserNotificationSettings>.Update;
        var updates = new List<UpdateDefinition<UserNotificationSettings>>();

        if (patchSubscriptionDto.IsEmailSubscribed is not null)
        {
            updates.Add(updateBuilder.Set(
                x => x.NotificationSubscription.IsEmailSubscribed,
                patchSubscriptionDto.IsEmailSubscribed.Value));
        }

        if (updates.Count == 0) return Task.CompletedTask;

        var combinedUpdate = updateBuilder.Combine(updates);

        return _notifications.UpdateOneAsync(
            item => item.Id == patchSubscriptionDto.Id,
            combinedUpdate,
            cancellationToken: cancellationToken);
    }
}
