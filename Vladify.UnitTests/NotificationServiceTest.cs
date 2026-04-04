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

    [Fact]
    public async Task GetNotifications_Should_ReturnNotifications_WhenValidInput()
    {
        var pageNumber = _fixture.Create<int>();
        var notifications = _fixture.Create<IEnumerable<NotificationInfo>>();
        var expectedModels = _fixture.Create<IEnumerable<NotificationModel>>();
        _repositoryMock.Setup(m => m.GetAllAsync(pageNumber, notifications.Count(), It.IsAny<CancellationToken>())).ReturnsAsync(notifications);
        _mapperMock.Setup(m => m.Map<IEnumerable<NotificationModel>>(notifications)).Returns(expectedModels);

        var result = await _notificationService.GetAllAsync(pageNumber, notifications.Count(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<NotificationModel>>();
        result.Count().Should().Be(expectedModels.Count());
        _repositoryMock.Verify(m => m.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /*[Theory]
    [InlineData(-1, 10)]
    [InlineData(-5, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(0, 10)]
    [InlineData(-1, -1)]
    public async Task GetNotifications_Should_ThrowArgumentException_WhenInputIsInvalid(int pageNumber, int pageSize)
    {
        var act = async () => await _notificationService.GetAllAsync(pageNumber, pageSize, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
        _repositoryMock.Verify(m => m.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }*/

    [Fact]
    public async Task GetByIdAsync_Should_ReturnModel_WhenValidInput()
    {
        var id = _fixture.Create<string>();
        var notification = _fixture.Create<NotificationInfo>();
        var expectedModel = _fixture.Create<NotificationModel>();
        _repositoryMock.Setup(m => m.GetByIdAsync(id, CancellationToken.None)).ReturnsAsync(notification);
        _mapperMock.Setup(m => m.Map<NotificationModel>(notification)).Returns(expectedModel);

        var result = await _notificationService.GetByIdAsync(id, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().NotBeEmpty();
        result.Should().BeOfType<NotificationModel>();
        _repositoryMock.Verify(m => m.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_WhenNotFound()
    {
        var id = _fixture.Create<string>();
        _repositoryMock.Setup(m => m.GetByIdAsync(id, CancellationToken.None)).ReturnsAsync((NotificationInfo?)null);
        _mapperMock.Setup(m => m.Map<NotificationModel>(It.IsAny<NotificationInfo>())).Returns((NotificationModel)null!);

        var result = await _notificationService.GetByIdAsync(id, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(m => m.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
