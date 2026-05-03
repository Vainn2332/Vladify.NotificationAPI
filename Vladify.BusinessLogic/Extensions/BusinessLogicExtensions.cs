using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vladify.BusinessLogic.Consumers;
using Vladify.BusinessLogic.Factories;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.MapperProfiles;
using Vladify.BusinessLogic.Options;
using Vladify.BusinessLogic.Services;
using Vladify.DataAccess.Extensions;

namespace Vladify.BusinessLogic.Extensions;

public static class BusinessLogicExtensions
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddServices()
            .AddMapping()
            .AddRabbitMQ(configuration)
            .ConfigureOptions(configuration)
            .AddDataAccessLayer(configuration);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();

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
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitOptions = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()
            ?? throw new ArgumentException($"Failed to bind section {RabbitMqOptions.SectionName}!");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<EmailSenderConsumer>();
            x.AddConsumer<CreateUserNotificationSettingsConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitOptions.ServerHost, h =>
                {
                    h.Username(rabbitOptions.Username);
                    h.Password(rabbitOptions.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
