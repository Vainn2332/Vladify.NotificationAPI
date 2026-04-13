using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.MapperProfiles;
using Vladify.BuisnessLogic.Options;
using Vladify.DataAccess.Extensions;

namespace Vladify.BuisnessLogic.Extensions;

public static class BusinessLogicExtensions
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddServices()
            .AddMapping()
            .ConfigureOptions(configuration)
            .AddDataAccessLayer(configuration);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }

    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(NotificationProfile).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        return services;
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailNotificationOptions>(configuration.GetSection(EmailNotificationOptions.SectionName));

        return services;
    }
}
