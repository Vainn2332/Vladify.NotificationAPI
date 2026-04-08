using AutoMapper;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.MapperProfiles;
using Vladify.DataAccess.Extensions;
using Vladify.DataAccess.Options;

namespace Vladify.BuisnessLogic.Extensions;

public static class BusinessLogicExtensions
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddServices()
            .AddMapping()
            .AddFirebase(configuration)
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
        services.Configure<FirebaseOptions>(configuration.GetSection(FirebaseOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddFirebase(this IServiceCollection services, IConfiguration configuration)
    {
        var firebaseOptions = configuration.GetSection(FirebaseOptions.SectionName).Get<FirebaseOptions>();

        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(firebaseOptions!.KeyPath)
        });

        return services;
    }
}