using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.MapperProfiles;

namespace Vladify.BuisnessLogic.Extensions;

public static class BusinessLogicExtensions
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services
            .AddServices()
            .AddMapping();

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
}
