using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Vladify.BusinessLogic.Exceptions;
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
            .AddJwtBasedAuthentication(configuration)
            .AddGraphQL();
    }

    public static IServiceCollection AddJwtBasedAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Options = configuration.GetSection(Auth0Options.SectionName).Get<Auth0Options>()
            ?? throw new NotFoundException($"Configuration section{Auth0Options.SectionName} not found!");

        var domain = auth0Options.Domain;
        var audience = auth0Options.Audience;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{domain}";

                options.Audience = audience;

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
