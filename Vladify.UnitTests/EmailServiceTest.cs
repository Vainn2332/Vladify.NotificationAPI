using AutoFixture;
using AutoFixture.AutoMoq;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using Vladify.BuisnessLogic;
using Vladify.BuisnessLogic.Interfaces;
using Vladify.BuisnessLogic.Models;
using Vladify.BuisnessLogic.Options;

namespace Vladify.UnitTests;

public class EmailServiceTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IOptions<EmailNotificationOptions>> _optionsMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ISmtpClientFactory> _factoryMock;
    private readonly Mock<ISmtpClient> _clientMock;
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly EmailService _emailService;

    public EmailServiceTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _optionsMock = _fixture.Freeze<Mock<IOptions<EmailNotificationOptions>>>();
        _notificationServiceMock = _fixture.Freeze<Mock<INotificationService>>();
        _factoryMock = _fixture.Freeze<Mock<ISmtpClientFactory>>();
        _clientMock = _fixture.Freeze<Mock<ISmtpClient>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<EmailService>>>();

        var fakeOptions = _fixture.Create<EmailNotificationOptions>();
        _optionsMock.Setup(m => m.Value).Returns(fakeOptions);
        _factoryMock.Setup(m => m.CreateClientAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_clientMock.Object);

        _emailService = _fixture.Create<EmailService>();
    }

    [Theory]
    [InlineData(40)]
    [InlineData(55)]
    [InlineData(81)]
    public async Task SendToAllUsersAsync_ShouldSendToAll_WhenValidInput(int dataAmount)
    {
        var models = _fixture.CreateMany<NotificationModel>(dataAmount);
        int expectedAmountOfCalls = models.Where(m => m.NotificationSubscription.Email == true).Count();
        var expectedChunks = (int)Math.Ceiling(dataAmount / 20.0);
        _notificationServiceMock
            .SetupSequence(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(models)
            .ReturnsAsync(new List<NotificationModel>());

        await _emailService.SendToAllUsersAsync("sub", "mes", CancellationToken.None);

        _factoryMock.Verify(m => m.CreateClientAsync(It.IsAny<CancellationToken>()), Times.Exactly(expectedChunks));
        _clientMock.Verify(m => m.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), default), Times.Exactly(expectedAmountOfCalls));
        _clientMock.Verify(m => m.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Exactly(expectedChunks));
    }

    [Fact]
    public async Task SendToAllUsersAsync_ShouldContinueProcessing_WhenOneChunkFails()
    {
        var models = _fixture.CreateMany<NotificationModel>(40).ToList();
        models.ForEach(m => m.NotificationSubscription.Email = true);
        _notificationServiceMock
            .SetupSequence(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(models)
            .ReturnsAsync(new List<NotificationModel>());
        _factoryMock
            .SetupSequence(f => f.CreateClientAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Failed to Create Smtp Client!"))
            .ReturnsAsync(_clientMock.Object);

        await _emailService.SendToAllUsersAsync("Sub", "Msg", CancellationToken.None);

        _loggerMock.Verify(m => m.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error happened while trying to notify user via email")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
        ), Times.Once);
        _clientMock.Verify(m => m.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Exactly(20));
    }
}
