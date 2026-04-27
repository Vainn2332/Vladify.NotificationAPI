using AutoFixture;
using AutoFixture.AutoMoq;
using MassTransit;
using Moq;
using Vladify.BuisnessLogic.Consumers;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Messages;

namespace Vladify.UnitTests;

public class NotificationConsumerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ConsumeContext<SongCreatedMessage>> _contextMock;
    private readonly NotificationConsumer _consumer;

    public NotificationConsumerTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _emailServiceMock = _fixture.Freeze<Mock<IEmailService>>();
        _contextMock = _fixture.Freeze<Mock<ConsumeContext<SongCreatedMessage>>>();
        _consumer = _fixture.Create<NotificationConsumer>();
    }

    [Fact]
    public async Task Consume_ShouldCallEmailService_WhenValidMessage()
    {
        var message = _fixture.Create<SongCreatedMessage>();
        var expectedBody = @$"<p>{message.Author} has posted {message.Title} in his album {message.Album}!</p>
            <p>Don't forget to check it up</p>";
        _contextMock.Setup(m => m.Message).Returns(message);
        _emailServiceMock.Setup(m => m.SendToAllUsersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _consumer.Consume(_contextMock.Object);

        _emailServiceMock.Verify(m => m.SendToAllUsersAsync(It.IsAny<string>(), expectedBody, It.IsAny<CancellationToken>()), Times.Once);
    }
}
