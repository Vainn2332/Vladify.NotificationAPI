using System.Text.Json.Serialization;
using Vladify.BusinessLogic.Models;
using static Vladify.IntegrationTests.GraphQL.ClientDtos.ClientDtos;

namespace Vladify.IntegrationTests.GraphQL.Responses;

public class QueryResponses
{
    public record NotificationByIdQueryResponse(
        [property: JsonPropertyName("notificationById")] UserNotificationSettingsModel Result
    );

    public record NotificationsQueryResponse(
        [property: JsonPropertyName("notifications")] List<UserNotificationSettingsModel> Result
    );

    public record PartialEmailSubscribersQueryResponse(
        [property: JsonPropertyName("emailSubscribers")] List<PartialSubscriberDto> Result
    );
}
