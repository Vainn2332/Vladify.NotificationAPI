using System.Text.Json.Serialization;
using static Vladify.IntegrationTests.GraphQl.Dtos.ClientDtos;

namespace Vladify.IntegrationTests.GraphQL.Responses;

public class MutationResponses
{
    public record PartialUpdateNotificationSettingsMutationResponse(
        [property: JsonPropertyName("updateNotificationSettings")] PartialSubscriberDto Result
    );

    public record PatchSubscriptionMutationResponse(
        [property: JsonPropertyName("patchSubscription")] PartialSubscriberDto Result
    );
}