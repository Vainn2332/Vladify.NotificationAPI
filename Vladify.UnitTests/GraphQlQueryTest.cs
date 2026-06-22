using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Vladify.BusinessLogic.Interfaces;
using Vladify.NotificationAPI.GraphQL.Queries;

namespace Vladify.UnitTests;

public class GraphQlQueryTest
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly IFixture _fixture;
    private readonly NotificationSettingsQuery _sut;

    public GraphQlQueryTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _notificationServiceMock = new Mock<INotificationService>();
        _sut = _fixture.Create<NotificationSettingsQuery>();
    }

    [Fact]
    public async Task GetNotificationById_ShouldReturnEntity_WhenValidInput()
    {
        var request = _fixture.Create<string>();

    }
}
