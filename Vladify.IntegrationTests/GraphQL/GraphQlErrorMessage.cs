using System.Text.Json.Serialization;

namespace Vladify.IntegrationTests.GraphQL;

public class GraphQlErrorMessage
{
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
