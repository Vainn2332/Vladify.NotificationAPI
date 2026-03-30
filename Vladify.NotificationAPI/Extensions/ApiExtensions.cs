using Vladify.BuisnessLogic.Options;

namespace Vladify.NotificationAPI.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));

        return services;
    }
}
