using AutoMapper;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vladify.BuisnessLogic.Consumers;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.MapperProfiles;
using Vladify.BuisnessLogic.Models.Messages;
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
            .AddKafka(configuration)
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
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaOptions = configuration.GetSection(KafkaOptions.SectionName).Get<KafkaOptions>()
            ?? throw new ArgumentException("Failed to get kafkaOptions from configuration!");

        services.AddMassTransit(options =>
        {
            options.UsingInMemory((context, config) =>
            {
                config.ConfigureEndpoints(context);
            });

            options.AddRider(rider =>
            {
                rider.AddConsumer<SongConsumer>();

                rider.UsingKafka((context, factory) =>
                {
                    factory.Host(kafkaOptions.ServerHost);

                    factory.TopicEndpoint<SongMessage>(kafkaOptions.Topics.SongCreated, kafkaOptions.ConsumerGroups.EmailService, config =>
                    {
                        config.ConfigureConsumer<SongConsumer>(context);

                        config.AutoOffsetReset = AutoOffsetReset.Earliest;

                        config.CreateIfMissing();
                    });
                });
            });
        });

        return services;
    }
}
