using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vladify.DataAccess.Entities;

public class NotificationInfo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public required Guid UserId { get; set; }

    public required string EmailAddress { get; set; }

    public required string FirebaseCloudMessagingToken { get; set; }

    public required NotificationSubscription NotificationSubscription { get; set; }
}
