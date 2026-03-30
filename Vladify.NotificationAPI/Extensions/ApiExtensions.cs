using MongoDB.Driver;
using Vladify.BuisnessLogic.Exceptions;
using Vladify.BuisnessLogic.Options;

namespace Vladify.NotificationAPI.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = configuration.GetSection(MongoDbOptions.SectionName).Get<MongoDbOptions>()
                ?? throw new NotFoundException($"Section{MongoDbOptions.SectionName} not found!");

            return new MongoClient(settings.ConnectionString);
        });


        return services;
    }

}
