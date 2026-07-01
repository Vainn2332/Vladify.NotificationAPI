using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Driver;
using Moq;
using Testcontainers.MongoDb;
using Vladify.IntegrationTests.Constants;

namespace Vladify.IntegrationTests.Infrastructure;

public class IntegrationTestInfrastructure : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();
    private IMongoDatabase _database = null!;
    private HttpClient _httpClient = null!;

    public GraphQlClient GraphQlClient { get; private set; } = null!;
    public TestDataSeeder Seeder { get; private set; } = null!;
    internal WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();

        var mongoClient = new MongoClient(_mongoDbContainer.GetConnectionString());
        _database = mongoClient.GetDatabase(TestConstants.DbName);

        Seeder = new TestDataSeeder(_database);

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["MongoDbOptions:ConnectionString"] = _mongoDbContainer.GetConnectionString(),
                    ["MongoDbOptions:DatabaseName"] = TestConstants.DbName
                });
            });

            builder.ConfigureServices(services =>
            {
                ConfigureTestServices(services);

                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = TestConstants.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = TestConstants.Issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(JwtBuilder.TestSecretKey))
                });
            });
        });

        _httpClient = Factory.CreateClient();
        GraphQlClient = new GraphQlClient(_httpClient);
    }

    private static void ConfigureTestServices(IServiceCollection services)
    {
        services
            .RemoveAll<IPublishEndpoint>();

        var publishEndpointMock = new Mock<IPublishEndpoint>();
        publishEndpointMock
            .Setup(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        services.AddScoped(serviceProvider => publishEndpointMock.Object);
    }

    public async Task DisposeAsync()
    {
        _httpClient.Dispose();
        await Factory.DisposeAsync();
        await _mongoDbContainer.DisposeAsync();
    }
}
