using AutoFixture;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Vladify.DataAccess.Entities;
using Vladify.IntegrationTests.Constants;
using static Vladify.IntegrationTests.GraphQL.Responses.QueryResponses;

namespace Vladify.IntegrationTests.GraphQL.Tests;

[Collection("FixtureCollection")]
public class GraphQlNotificationSettingsQueryTest
{
    private readonly IFixture _fixture;
    private readonly IntegrationTestInfrastructure _infrastructure;
    private readonly JsonSerializerOptions _serializerOptions;
    public GraphQlNotificationSettingsQueryTest(IntegrationTestInfrastructure infrastructure)
    {
        _fixture = AutoFixtureOptions.CreateFixture();
        _infrastructure = infrastructure;
        _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ShouldReturnEntity_WhenValidInput()
    {
        await _infrastructure.ResetDataAsync();

        var userSettings = _fixture.Create<UserNotificationSettings>();
        var seededEntity = await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, userSettings);

        var query = $$"""
            query {
                notificationById(id: "{{seededEntity.Id}}") {
                    id
                    userId
                    emailAddress
                    notificationSubscription {
                        isEmailSubscribed
                    }
                }
            }
            """;

        var token = _infrastructure.GenerateTestJWT(seededEntity.EmailAddress);
        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _infrastructure.Client.SendAsync(request);

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<NotificationByIdQueryResponse>>(_serializerOptions);

        result!.Data!.Result.Should().NotBeNull();
        result!.Errors.Should().BeEmpty();

        result!.Data.Result!.Id.Should().Be(seededEntity.Id);
        result!.Data.Result!.NotificationSubscription.IsEmailSubscribed
            .Should().Be(seededEntity.NotificationSubscription.IsEmailSubscribed);
    }

    [Fact]
    public async Task GetNotificationByIdAsync_ShouldReturnUnauthorizedException_WhenUnauthorized()
    {
        await _infrastructure.ResetDataAsync();

        var userSettings = _fixture.Create<UserNotificationSettings>();
        var seededEntity = await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, userSettings);

        var query = $$"""
            query {
                notificationById(id: "{{seededEntity.Id}}") {
                    id
                    userId
                    emailAddress
                    notificationSubscription {
                        isEmailSubscribed
                    }
                }
            }
            """;

        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query })
        };
        using var response = await _infrastructure.Client.SendAsync(request);

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<NotificationByIdQueryResponse>>(_serializerOptions);

        result!.Data!.Result.Should().BeNull();
        result!.Errors.Should().NotBeEmpty();

        result!.Errors[0].Message.Should().Be("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task GetNotificationsAsync_ShouldReturnList_WhenValidInput()
    {
        await _infrastructure.ResetDataAsync();

        var entities = _fixture.CreateMany<UserNotificationSettings>(3).ToList();
        foreach (var entity in entities)
        {
            await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, entity);
        }

        var query = $$"""
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

        var token = _infrastructure.GenerateTestJWT(entities[0].EmailAddress);
        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _infrastructure.Client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<NotificationsQueryResponse>>(_serializerOptions);

        result!.Errors.Should().BeEmpty();
        result!.Data!.Result.Should().NotBeNull();

        var returnedList = result.Data.Result;
        returnedList.Should().HaveCount(3);
        returnedList!.Select(x => x.Id).Should().Contain(entities.Select(e => e.Id));
    }

    [Fact]
    public async Task GetEmailSubscribersAsync_ShouldReturnPartialSubscribersDto_WhenValidInput()
    {
        await _infrastructure.ResetDataAsync();

        var subscriber = _fixture.Create<UserNotificationSettings>();
        subscriber.NotificationSubscription.IsEmailSubscribed = true;

        var unsubscribedUser = _fixture.Create<UserNotificationSettings>();
        unsubscribedUser.NotificationSubscription.IsEmailSubscribed = false;

        await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, subscriber);
        await _infrastructure.SeedDataAsync(TestConstants.UserNotificationSettingsCollectionName, unsubscribedUser);

        var query = """
            query {
                emailSubscribers(pageNumber: 1, pageSize: 10) {
                    id
                    emailAddress
                }
            }
            """;

        var token = _infrastructure.GenerateTestJWT(subscriber.EmailAddress);
        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _infrastructure.Client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<PartialEmailSubscribersQueryResponse>>(_serializerOptions);

        result!.Errors.Should().BeEmpty();
        result!.Data!.Result.Should().NotBeNull();
        result!.Data.Result.Count.Should().Be(1);
    }
}
