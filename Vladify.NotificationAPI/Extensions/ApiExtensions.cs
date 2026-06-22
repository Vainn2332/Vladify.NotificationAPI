using Vladify.BusinessLogic.Extensions;
using Vladify.NotificationAPI.GraphQL;

namespace Vladify.NotificationAPI.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddBusinessLogicLayer(configuration)
            .AddGraphQL();
    }

    public static IServiceCollection AddGraphQL(this IServiceCollection services)
    {
        services
           .AddGraphQLServer()
           .AddQueryType<Query>()
           .AddMutationType<Mutation>();

        return services;
    }
}
