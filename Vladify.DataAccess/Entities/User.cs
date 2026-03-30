using MongoDB.Bson.Serialization.Attributes;

namespace Vladify.DataAccess.Entities;

public class User
{
    [BsonId]
    public int Id { get; set; }

    public required string UserId { get; set; }

    public required NotificationSubscription NotificationSubscription { get; set; }
}
