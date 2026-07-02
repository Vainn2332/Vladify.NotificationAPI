using System.Text.Json.Serialization;
using Vladify.BusinessLogic.Models;
using static Vladify.IntegrationTests.GraphQl.Dtos.ClientDtos;

namespace Vladify.IntegrationTests.GraphQL.Responses;

public class QueryResponses
{
    public record NotificationByIdQueryResponse(
        [property: JsonPropertyName("notificationById")] UserNotificationSettingsModel Result
    );

    public record NotificationsQueryResponse(
        [property: JsonPropertyName("notifications")] UserNotificationSettingsModel[] Result
    );

    public record PartialEmailSubscribersQueryResponse(
        [property: JsonPropertyName("emailSubscribers")] PartialSubscriberDto[] Result
    );
}
