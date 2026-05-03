using AutoFixture;
using AutoFixture.AutoMoq;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Vladify.BusinessLogic.Factories;
using Vladify.BusinessLogic.Options;

namespace Vladify.UnitTests;

public class SmtpClientFactoryTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ISmtpClient> _clientMock;
    private readonly Mock<IOptions<EmailNotificationOptions>> _optionsMock;
    private readonly SmtpClientFactory _factory;

    public SmtpClientFactoryTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _clientMock = _fixture.Freeze<Mock<ISmtpClient>>();

        var options = _fixture.Create<EmailNotificationOptions>();
        _optionsMock = _fixture.Create<Mock<IOptions<EmailNotificationOptions>>>();
        _optionsMock.Setup(m => m.Value).Returns(options);

        _factory = _fixture.Create<SmtpClientFactory>();
    }

    [Fact]
    public async Task CheckForConnectionAsync_ShouldDoNothing_WhenClientIsAlreadyConnected()
    {
        _clientMock.Setup(c => c.IsConnected).Returns(true);

        await _factory.EnsureConnectedAsync(_clientMock.Object, CancellationToken.None);

        _clientMock.Verify(m => m.ConnectAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<SecureSocketOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CheckForConnectionAsync_ShouldReconnectAndAuth_WhenClientIsDisconnected()
    {
        _clientMock.Setup(c => c.IsConnected).Returns(false);

        await _factory.EnsureConnectedAsync(_clientMock.Object, CancellationToken.None);

        _clientMock.Verify(m => m.ConnectAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<SecureSocketOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _clientMock.Verify(c => c.AuthenticateAsync(
            It.IsAny<NetworkCredential>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
