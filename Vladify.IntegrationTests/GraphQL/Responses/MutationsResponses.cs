using System.Text.Json.Serialization;
using Vladify.BusinessLogic.Models;

namespace Vladify.IntegrationTests.GraphQL.Responses;

public class MutationsResponses
{
    public record UpdateNotificationSettingsMutationResponse(
    [property: JsonPropertyName("updateNotificationSettings")] UserNotificationSettingsModel Result
    );

    public record PatchSubscriptionMutationResponse(
        [property: JsonPropertyName("patchSubscription")] bool Result
    );
}
