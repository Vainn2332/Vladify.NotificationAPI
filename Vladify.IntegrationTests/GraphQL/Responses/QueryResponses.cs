using System.Text.Json.Serialization;
using Vladify.BusinessLogic.Models;

namespace Vladify.IntegrationTests.GraphQL.Responses;

internal class QueryResponses
{
    public record NotificationByIdQueryResponse(
    [property: JsonPropertyName("notificationById")] UserNotificationSettingsModel? Result
    );

    public record NotificationsQueryResponse(
        [property: JsonPropertyName("notifications")] List<UserNotificationSettingsModel> Result
    );

    public record EmailSubscribersQueryResponse(
        [property: JsonPropertyName("emailSubscribers")] List<UserNotificationSettingsModel> Result
    );
}
