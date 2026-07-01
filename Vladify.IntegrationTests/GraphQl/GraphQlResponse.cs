namespace Vladify.IntegrationTests.GraphQL;

public class GraphQlResponse<TQueryResponse>
{
    public TQueryResponse? Data { get; set; }

    public IReadOnlyList<GraphQlErrorMessage>? Errors { get; set; } = [];
}