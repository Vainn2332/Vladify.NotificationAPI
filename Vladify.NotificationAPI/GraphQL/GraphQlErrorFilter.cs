namespace Vladify.NotificationAPI.GraphQL;

public class GraphQlErrorFilter(ILogger<GraphQlErrorFilter> _logger) : IErrorFilter
{
    public IError OnError(IError error)
    {
        _logger.LogError(error.Exception, "GraphQL Unhandled exception occured!");

        return error;
    }
}
