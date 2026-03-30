using MongoDB.Driver;
using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess;

public class NotificationRepository
{
    private readonly IMongoCollection<NotificationInfo> _notifications;

    public NotificationRepository(IMongoCollection<NotificationInfo> notifications)
    {
        _notifications = notifications;
    }


}
