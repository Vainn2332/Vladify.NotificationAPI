using System.Text.Json.Serialization;
using static Vladify.IntegrationTests.GraphQL.ClientDtos.ClientDtos;

namespace Vladify.IntegrationTests.GraphQL.Responses;

public class MutationsResponses
{
    public record PartialUpdateNotificationSettingsMutationResponse(
        [property: JsonPropertyName("updateNotificationSettings")] PartialSubscriberDto Result
    );

    public record PatchSubscriptionMutationResponse(
        [property: JsonPropertyName("patchSubscription")] bool Result
    );
}
