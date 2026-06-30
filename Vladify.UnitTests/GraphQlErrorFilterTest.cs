using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using HotChocolate;
using Microsoft.Extensions.Logging;
using Moq;
using Vladify.NotificationAPI.GraphQL;

namespace Vladify.UnitTests;

public class GraphQlErrorFilterTest
{
    private readonly IFixture _fixture;
    private readonly Mock<ILogger<GraphQlErrorFilter>> _loggerMock;
    private readonly GraphQlErrorFilter _filter;


    public GraphQlErrorFilterTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _loggerMock = _fixture.Freeze<Mock<ILogger<GraphQlErrorFilter>>>();
        _filter = _fixture.Create<GraphQlErrorFilter>();
    }

    [Fact]
    public void OnError_ShouldLogException_AndReturnSameError()
    {
        var expectedException = new Exception("Test GraphQL exception");
        var errorMock = new Mock<IError>();
        errorMock.Setup(e => e.Exception).Returns(expectedException);

        var result = _filter.OnError(errorMock.Object);

        result.Should().Be(errorMock.Object);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("GraphQL Unhandled exception occured!")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
