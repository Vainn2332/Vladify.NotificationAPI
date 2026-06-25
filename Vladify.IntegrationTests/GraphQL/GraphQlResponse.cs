namespace Vladify.IntegrationTests.GraphQL;

public class GraphQlResponse<TQueryResponse>
{
    public TQueryResponse? Data { get; set; }

    public GraphQlErrorMessage[]? Errors { get; set; } = [];
}
