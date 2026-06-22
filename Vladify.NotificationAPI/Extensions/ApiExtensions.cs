using Vladify.BusinessLogic.Extensions;
using Vladify.NotificationAPI.Constants;
using Vladify.NotificationAPI.GraphQL.Mutations;
using Vladify.NotificationAPI.GraphQL.Queries;

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
           .AddQueryType(q => q.Name(GraphQlConstants.QueryName))
           .AddMutationType(q => q.Name(GraphQlConstants.MutationName))
           .AddTypeExtension<NotificationSettingsQuery>()
           .AddTypeExtension<NotificationSettingsMutation>();

        return services;
    }
}
