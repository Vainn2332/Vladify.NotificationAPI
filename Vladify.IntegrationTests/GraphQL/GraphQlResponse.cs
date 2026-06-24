using System.Text.Json.Serialization;

namespace Vladify.IntegrationTests.GraphQL;

public class GraphQlResponse<TClass>
{
    [JsonPropertyName("data")]
    public required TClass Data { get; set; }

    [JsonPropertyName("errors")]
    public required GraphQlErrorMessage[] Errors { get; set; } = [];
}
