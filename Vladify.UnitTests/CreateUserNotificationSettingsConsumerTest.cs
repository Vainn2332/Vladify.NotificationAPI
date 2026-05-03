using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using MassTransit;
using Moq;
using Vladify.BusinessLogic.Consumers;
using Vladify.BusinessLogic.Interfaces;
using Vladify.BusinessLogic.Messages;
using Vladify.BusinessLogic.Models;

namespace Vladify.UnitTests;

public class CreateUserNotificationSettingsConsumerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ConsumeContext<UserCreatedMessage>> _contextMock;
    private readonly CreateUserNotificationSettingsConsumer _consumer;

    public CreateUserNotificationSettingsConsumerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _notificationServiceMock = _fixture.Freeze<Mock<INotificationService>>();
        _contextMock = _fixture.Freeze<Mock<ConsumeContext<UserCreatedMessage>>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _consumer = _fixture.Create<CreateUserNotificationSettingsConsumer>();
    }

    [Fact]
    public async Task Consume_ShouldCreateUserInDb_WhenValidMessage()
    {
        var message = _fixture.Create<UserCreatedMessage>();
        var requestModel = _fixture.Create<UserNotificationSettingsRequestModel>();
        var expectedModel = _fixture.Create<UserNotificationSettingsModel>();
        _contextMock.Setup(m => m.Message).Returns(message);
        _mapperMock.Setup(m => m.Map<UserNotificationSettingsRequestModel>(message)).Returns(requestModel);
        _notificationServiceMock.Setup(m => m.CreateAsync(requestModel, It.IsAny<CancellationToken>())).ReturnsAsync(expectedModel);

        await _consumer.Consume(_contextMock.Object);

        _notificationServiceMock.Verify(m => m.CreateAsync(requestModel, It.IsAny<CancellationToken>()), Times.Once);
    }
}
