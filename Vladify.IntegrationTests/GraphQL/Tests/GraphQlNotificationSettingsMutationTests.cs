using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Vladify.BusinessLogic.Interfaces;
using Vladify.DataAccess.Entities;
using Vladify.IntegrationTests.Constants;
using static Vladify.IntegrationTests.GraphQL.Responses.MutationsResponses;

namespace Vladify.IntegrationTests.GraphQL.Tests;

[Collection("FixtureCollection")]
public class GraphQlNotificationSettingsMutationTests
{
    private readonly IFixture _fixture;
    private readonly IntegrationTestInfrastructure _infrastructure;
    private readonly JsonSerializerOptions _serializerOptions;

    public GraphQlNotificationSettingsMutationTests(IntegrationTestInfrastructure infrastructure)
    {
        _fixture = AutoFixtureOptions.CreateFixture();
        _infrastructure = infrastructure;
        _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    [Fact]
    public async Task UpdateNotificationSettingsAsync_ShouldUpdateEntityAndReturnIt()
    {
        await _infrastructure.ResetDataAsync();

        var originalEntity = _fixture.Create<UserNotificationSettings>();
        await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, originalEntity);

        var newEmail = "updated_email@example.com";

        var mutation = $$"""
            mutation {
                updateNotificationSettings(input: {
                    id: "{{originalEntity.Id}}",
                    userId: "{{originalEntity.UserId}}",
                    emailAddress: "{{newEmail}}",
                    notificationSubscription: {
                        isEmailSubscribed: {{originalEntity.NotificationSubscription.IsEmailSubscribed.ToString().ToLower()}}
                    }
                }) {
                    id
                    emailAddress
                }
            }
            """;

        var token = IntegrationTestInfrastructure.GenerateTestJWT(originalEntity.EmailAddress);
        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query = mutation })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _infrastructure.Client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<PartialUpdateNotificationSettingsMutationResponse>>(_serializerOptions);

        using var scope = _infrastructure.Factory.Services.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var entityInDb = await notificationService.GetByIdAsync(originalEntity.Id, CancellationToken.None);

        result!.Errors.Should().BeEmpty();
        result.Data?.Result.Should().NotBeNull();
        result.Data?.Result.EmailAddress.Should().Be(newEmail);

        entityInDb.Should().NotBeNull();
        entityInDb!.EmailAddress.Should().Be(newEmail);
    }

    [Fact]
    public async Task PatchSubscriptionAsync_ShouldUpdateOnlySubscriptionStatus()
    {
        await _infrastructure.ResetDataAsync();

        var originalEntity = _fixture.Create<UserNotificationSettings>();
        originalEntity.NotificationSubscription.IsEmailSubscribed = false;

        await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, originalEntity);

        var newSubscriptionStatus = true;

        var mutation = $$"""
            mutation {
                patchSubscription(input: {
                    id: "{{originalEntity.Id}}",
                    isEmailSubscribed: {{newSubscriptionStatus.ToString().ToLower()}}
                })
            }
            """;

        var token = IntegrationTestInfrastructure.GenerateTestJWT(originalEntity.EmailAddress);
        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query = mutation })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _infrastructure.Client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<PatchSubscriptionMutationResponse>>(_serializerOptions);

        using var scope = _infrastructure.Factory.Services.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var entityInDb = await notificationService.GetByIdAsync(originalEntity.Id, CancellationToken.None);

        result!.Errors.Should().BeEmpty();
        result!.Data!.Result.Should().BeTrue();

        entityInDb.Should().NotBeNull();
        entityInDb.EmailAddress.Should().Be(originalEntity.EmailAddress);
        entityInDb.NotificationSubscription.IsEmailSubscribed.Should().Be(newSubscriptionStatus);
    }

    [Fact]
    public async Task PatchSubscriptionAsync_ShouldNotUpdateAnything_WhenPatchParamsAreNull()
    {
        await _infrastructure.ResetDataAsync();

        var originalEntity = _fixture.Create<UserNotificationSettings>();
        originalEntity.NotificationSubscription.IsEmailSubscribed = false;

        await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, originalEntity);


        var mutation = $$"""
            mutation {
                patchSubscription(input: {
                    id: "{{originalEntity.Id}}",
                    isEmailSubscribed: null
                })
            }
            """;

        var token = IntegrationTestInfrastructure.GenerateTestJWT(originalEntity.EmailAddress);
        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query = mutation })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _infrastructure.Client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<PatchSubscriptionMutationResponse>>(_serializerOptions);

        using var scope = _infrastructure.Factory.Services.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var entityInDb = await notificationService.GetByIdAsync(originalEntity.Id, CancellationToken.None);

        result!.Errors.Should().BeEmpty();
        result!.Data!.Result.Should().BeTrue();

        entityInDb.Should().NotBeNull();
        entityInDb.EmailAddress.Should().Be(originalEntity.EmailAddress);
        entityInDb.NotificationSubscription.IsEmailSubscribed.Should().Be(originalEntity.NotificationSubscription.IsEmailSubscribed);
    }
}
