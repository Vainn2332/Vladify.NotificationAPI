using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Vladify.DataAccess.Constants;
using Vladify.DataAccess.Entities;
using Vladify.DataAccess.Options;

namespace Vladify.DataAccess.Extensions;

public static class DataAccessExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMongoDb()
            .ConfigureOptions(configuration);

        return services;
    }
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var mongoDbOptions = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;

            return new MongoClient(mongoDbOptions.ConnectionString);
        });

        services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var mongoDbOptions = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var database = client.GetDatabase(mongoDbOptions.DatabaseName);

            return database.GetCollection<UserNotificationSettings>(DataAccessConstants.CollectionName);
        });

        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));

        return services;
    }
}
