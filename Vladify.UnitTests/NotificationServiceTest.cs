using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
using Vladify.BuisnessLogic;
using Vladify.BuisnessLogic.Models;
using Vladify.DataAccess;
using Vladify.DataAccess.Entities;

namespace Vladify.UnitTests;

public class NotificationServiceTest
{
    private readonly IFixture _fixture;
    private readonly Mock<INotificationRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly NotificationService _notificationService;
    public NotificationServiceTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _repositoryMock = _fixture.Freeze<Mock<INotificationRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _notificationService = _fixture.Create<NotificationService>();
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnModel_WhenValidInputAsync()
    {
        var request = _fixture.Create<NotificationRequestModel>();
        var notification = _fixture.Create<NotificationInfo>();
        var newNotification = _fixture.Create<NotificationInfo>();
        var expectedModel = _fixture.Create<NotificationModel>();
        _mapperMock.Setup(m => m.Map<NotificationInfo>(request)).Returns(notification);
        _repositoryMock.Setup(m => m.GetByUserIdAsync(request.UserId, CancellationToken.None)).ReturnsAsync((NotificationInfo?)null);
        _repositoryMock.Setup(m => m.CreateAsync(notification, CancellationToken.None)).ReturnsAsync(newNotification);
        _mapperMock.Setup(m => m.Map<NotificationModel>(newNotification)).Returns(expectedModel);

        var result = await _notificationService.CreateAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNull();
        result.Should().BeOfType<NotificationModel>();
        _repositoryMock.Verify(m => m.CreateAsync(It.IsAny<NotificationInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnArgumentException_WhenUserIdAlreadyExists()
    {
        var request = _fixture.Create<NotificationRequestModel>();

        var act = async () => await _notificationService.CreateAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("notification with such user already exists!");
        _repositoryMock.Verify(m => m.CreateAsync(It.IsAny<NotificationInfo>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
