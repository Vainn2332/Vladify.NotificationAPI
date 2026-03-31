using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Vladify.DataAccess.Constants;
using Vladify.DataAccess.Entities;

namespace Vladify.DataAccess.Extensions;

public static class DataAccessExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration);

        return services;
    }
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var connectionString = configuration.GetConnectionString(DataAccessConstants.DatabaseName)
                ?? throw new InvalidOperationException($"Connection string 'MongoDb' not found!");

            return new MongoClient(connectionString);
        });

        services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase(DataAccessConstants.DatabaseName);

            return database.GetCollection<NotificationInfo>(DataAccessConstants.CollectionName);
        });

        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
