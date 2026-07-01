using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vladify.BusinessLogic.Extensions;
using Vladify.BusinessLogic.Options;
using Vladify.NotificationAPI.Constants;
using Vladify.NotificationAPI.GraphQL;
using Vladify.NotificationAPI.GraphQL.Mutations;
using Vladify.NotificationAPI.GraphQL.Queries;

namespace Vladify.NotificationAPI.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddBusinessLogicLayer(configuration)
            .AddJwtBasedAuthentication()
            .AddGraphQL();
    }

    public static IServiceCollection AddJwtBasedAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
        .Configure<IOptions<Auth0Options>>((options, auth0) =>
        {
            var auth0Options = auth0.Value;

            options.Authority = auth0Options.Authority;
            options.Audience = auth0Options.Audience;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        return services;
    }

    public static IServiceCollection AddGraphQL(this IServiceCollection services)
    {
        services
           .AddGraphQLServer()
           .AddAuthorization()
           .AddQueryType(q => q.Name(GraphQlConstants.QueryName))
           .AddMutationType(q => q.Name(GraphQlConstants.MutationName))
           .AddTypeExtension<NotificationSettingsQuery>()
           .AddTypeExtension<NotificationSettingsMutation>()
           .AddErrorFilter<GraphQlErrorFilter>();

        return services;
    }
}
