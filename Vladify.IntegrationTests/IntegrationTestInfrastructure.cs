using AutoFixture;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Testcontainers.MongoDb;
using Vladify.IntegrationTests.Constants;

namespace Vladify.IntegrationTests;

public class IntegrationTestInfrastructure : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();
    private IMongoDatabase _database = null!;
    private readonly IFixture _fixture = new Fixture();
    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();

        var mongoClient = new MongoClient(_mongoDbContainer.GetConnectionString());
        _database = mongoClient.GetDatabase("TestDb");

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["MongoDbOptions:ConnectionString"] = _mongoDbContainer.GetConnectionString(),
                    ["MongoDbOptions:DatabaseName"] = "TestDb"
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestConstants.TestSecretKey))
                });
            });
        });

        Client = Factory.CreateClient();
    }

    public async Task ResetDataAsync()
    {
        var collections = await _database.ListCollectionNamesAsync();
        var collectionNames = await collections.ToListAsync();

        foreach (var collectionName in collectionNames)
        {
            await _database
                .GetCollection<BsonDocument>(collectionName)
                .DeleteManyAsync(Builders<BsonDocument>.Filter.Empty);
        }
    }

    public static string GenerateTestJWT(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestConstants.TestSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(TestConstants.CustomEmailClaimName,email)
        };

        var token = new JwtSecurityToken(
            issuer: TestConstants.Issuer,
            audience: TestConstants.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await _mongoDbContainer.DisposeAsync();
    }

    public async Task<TEntity> SeedDataAsync<TEntity>(string collectionName, TEntity entity) where TEntity : class
    {
        var collection = _database.GetCollection<TEntity>(collectionName);
        await collection.InsertOneAsync(entity);

        return entity;
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
}
