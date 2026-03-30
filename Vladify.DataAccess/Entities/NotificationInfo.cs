using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vladify.DataAccess.Entities;

public class NotificationInfo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    public required string UserId { get; set; }

    public required string EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public required NotificationSubscription NotificationSubscription { get; set; }
}
