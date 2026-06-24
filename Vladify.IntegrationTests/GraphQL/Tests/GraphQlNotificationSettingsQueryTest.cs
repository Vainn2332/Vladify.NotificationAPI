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

    public GraphQlNotificationSettingsQueryTest(IntegrationTestInfrastructure infrastructure)
    {
        _fixture = AutoFixtureOptions.CreateFixture();
        _infrastructure = infrastructure;
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

        var result = await response.Content.ReadFromJsonAsync<GraphQlResponse<NotificationByIdQueryResponse>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result!.Data.Should().NotBeNull();
        result!.Errors.Should().BeEmpty();

        result!.Data.Result!.Id.Should().Be(seededEntity.Id);
        result!.Data.Result!.NotificationSubscription.IsEmailSubscribed
            .Should().Be(seededEntity.NotificationSubscription.IsEmailSubscribed);
    }
}
