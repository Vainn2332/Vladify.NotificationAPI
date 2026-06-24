namespace Vladify.IntegrationTests.GraphQL;

public class GraphQlResponse<TClass>
{
    public TClass? Data { get; set; }

    public GraphQlErrorMessage[]? Errors { get; set; } = [];
}
