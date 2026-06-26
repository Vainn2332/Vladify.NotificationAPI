using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Models;
using Vladify.DataAccess.Entities;
using Vladify.IntegrationTests.Constants;
using Vladify.IntegrationTests.Infrastructure;
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
        await _infrastructure.Seeder.ResetDataAsync();

        var originalEntity = _fixture.Create<UserNotificationSettings>();
        await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, originalEntity);

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

        var result = await _infrastructure.GraphQlClient.SendAsync<PartialUpdateNotificationSettingsMutationResponse>(mutation, originalEntity.EmailAddress);
        var entityInDb = await GetEntityFromDbAsync(originalEntity.Id);

        result.Errors.Should().BeEmpty();
        result.Data?.Result.Should().NotBeNull();
        result.Data?.Result.EmailAddress.Should().Be(newEmail);
        entityInDb.Should().NotBeNull();
        entityInDb!.EmailAddress.Should().Be(newEmail);
    }

    [Fact]
    public async Task PatchSubscriptionAsync_ShouldUpdateOnlySubscriptionStatus()
    {
        await _infrastructure.Seeder.ResetDataAsync();

        var originalEntity = _fixture.Create<UserNotificationSettings>();
        originalEntity.NotificationSubscription.IsEmailSubscribed = false;
        await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, originalEntity);

        var newSubscriptionStatus = true;
        var mutation = BuildPatchSubscriptionMutation(originalEntity.Id, newSubscriptionStatus.ToString().ToLower());

        var result = await _infrastructure.GraphQlClient.SendAsync<PatchSubscriptionMutationResponse>(mutation, originalEntity.EmailAddress);
        var entityInDb = await GetEntityFromDbAsync(originalEntity.Id);

        result.Errors.Should().BeEmpty();
        result.Data!.Result.Should().NotBeNull();
        entityInDb.Should().NotBeNull();
        entityInDb!.EmailAddress.Should().Be(originalEntity.EmailAddress);
        entityInDb.NotificationSubscription.IsEmailSubscribed.Should().Be(newSubscriptionStatus);
    }

    [Fact]
    public async Task PatchSubscriptionAsync_ShouldNotUpdateAnything_WhenPatchParamsAreNull()
    {
        await _infrastructure.Seeder.ResetDataAsync();

        var originalEntity = _fixture.Create<UserNotificationSettings>();
        originalEntity.NotificationSubscription.IsEmailSubscribed = false;
        await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, originalEntity);

        var mutation = BuildPatchSubscriptionMutation(originalEntity.Id, "null");

        var result = await _infrastructure.GraphQlClient.SendAsync<PatchSubscriptionMutationResponse>(mutation, originalEntity.EmailAddress);
        var entityInDb = await GetEntityFromDbAsync(originalEntity.Id);

        result.Errors.Should().BeEmpty();
        result.Data!.Result.Should().NotBeNull();
        entityInDb.Should().NotBeNull();
        entityInDb!.EmailAddress.Should().Be(originalEntity.EmailAddress);
        entityInDb.NotificationSubscription.IsEmailSubscribed.Should().Be(originalEntity.NotificationSubscription.IsEmailSubscribed);
    }

    private static string BuildPatchSubscriptionMutation(string id, string isEmailSubscribedValue) =>
        $$"""
        mutation {
            patchSubscription(input: {
                id: "{{id}}",
                isEmailSubscribed: {{isEmailSubscribedValue}}
            }) {
                id
                emailAddress
            }
        }
        """;

    private Task<UserNotificationSettingsModel?> GetEntityFromDbAsync(string id)
    {
        using var scope = _infrastructure.Factory.Services.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        return notificationService.GetByIdAsync(id, CancellationToken.None);
    }
}