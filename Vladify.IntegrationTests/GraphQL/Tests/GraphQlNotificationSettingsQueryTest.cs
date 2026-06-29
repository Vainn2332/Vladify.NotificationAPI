using AutoFixture;
using FluentAssertions;
using Vladify.DataAccess.Entities;
using Vladify.IntegrationTests.Constants;
using Vladify.IntegrationTests.Infrastructure;
using static Vladify.IntegrationTests.GraphQL.Responses.QueryResponses;

namespace Vladify.IntegrationTests.GraphQL.Tests;

[Collection("FixtureCollection")]
public class GraphQlNotificationSettingsQueryTest
{
    private readonly IFixture _fixture;
    private readonly IntegrationTestInfrastructure _infrastructure;

    public GraphQlNotificationSettingsQueryTest(IntegrationTestInfrastructure infrastructure)
    {
        _fixture = AutoFixtureOptions.CreateFixture();
        _infrastructure = infrastructure;
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ShouldReturnEntity_WhenValidInput()
    {
        await _infrastructure.Seeder.ResetDataAsync();

        var userSettings = _fixture.Create<UserNotificationSettings>();
        var seededEntity = await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, userSettings);

        var query = BuildNotificationByIdQuery(seededEntity.Id);

        var result = await _infrastructure.GraphQlClient.SendAsync<NotificationByIdQueryResponse>(query, seededEntity.EmailAddress);

        result.Data!.Result.Should().NotBeNull();
        result.Errors.Should().BeEmpty();
        result.Data.Result!.Id.Should().Be(seededEntity.Id);
        result.Data.Result.NotificationSubscription.IsEmailSubscribed
            .Should().Be(seededEntity.NotificationSubscription.IsEmailSubscribed);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ShouldReturnUnauthorizedException_WhenUnauthorized()
    {
        await _infrastructure.Seeder.ResetDataAsync();

        var userSettings = _fixture.Create<UserNotificationSettings>();
        var seededEntity = await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, userSettings);

        var query = BuildNotificationByIdQuery(seededEntity.Id);

        var result = await _infrastructure.GraphQlClient.SendAsync<NotificationByIdQueryResponse>(query);

        result.Data!.Result.Should().BeNull();
        result.Errors.Should().NotBeEmpty();
        result.Errors![0].Message.Should().Be("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task GetNotificationsAsync_ShouldReturnList_WhenValidInput()
    {
        await _infrastructure.Seeder.ResetDataAsync();

        var entities = _fixture.CreateMany<UserNotificationSettings>(3).ToList();
        foreach (var entity in entities)
        {
            await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, entity);
        }

        var query = """
            query {
                notifications(pageNumber: 1, pageSize: 10) {
                    id
                    userId
                    emailAddress
                    notificationSubscription {
                        isEmailSubscribed
                    }
                }
            }
            """;

        var result = await _infrastructure.GraphQlClient.SendAsync<NotificationsQueryResponse>(query, entities[0].EmailAddress);

        result.Errors.Should().BeEmpty();
        result.Data!.Result.Should().NotBeNull();

        var returnedList = result.Data.Result;
        returnedList.Should().HaveCount(3);
        returnedList!.Select(x => x.Id).Should().Contain(entities.Select(e => e.Id));
    }

    [Fact]
    public async Task GetEmailSubscribersAsync_ShouldReturnPartialSubscribersDto_WhenValidInput()
    {
        await _infrastructure.Seeder.ResetDataAsync();

        var subscriber = _fixture.Create<UserNotificationSettings>();
        subscriber.NotificationSubscription.IsEmailSubscribed = true;

        var unsubscribedUser = _fixture.Create<UserNotificationSettings>();
        unsubscribedUser.NotificationSubscription.IsEmailSubscribed = false;

        await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, subscriber);
        await _infrastructure.Seeder.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, unsubscribedUser);

        var query = """
            query {
                emailSubscribers(pageNumber: 1, pageSize: 10) {
                    id
                    emailAddress
                }
            }
            """;

        var result = await _infrastructure.GraphQlClient.SendAsync<PartialEmailSubscribersQueryResponse>(query, subscriber.EmailAddress);

        result.Errors.Should().BeEmpty();
        result.Data!.Result.Should().NotBeNull();
        result.Data.Result.Length.Should().Be(1);
    }

    private static string BuildNotificationByIdQuery(string id) =>
        $$"""
           query {
               notificationById(id: "{{id}}") {
                   id
                   userId
                   emailAddress
                   notificationSubscription {
                       isEmailSubscribed
                   }                
               }
           }
         """;
}
