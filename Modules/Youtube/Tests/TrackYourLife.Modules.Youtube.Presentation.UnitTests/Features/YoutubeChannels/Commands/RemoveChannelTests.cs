using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Commands;

public class RemoveChannelTests
{
    private readonly ISender _sender;
    private readonly RemoveChannel _endpoint;

    public RemoveChannelTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new RemoveChannel(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        _sender
            .Send(Arg.Any<RemoveChannelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        var youtubeChannelId = "UCtest123456789";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", youtubeChannelId } };
        _endpoint.SetHttpContext(httpContext);

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<RemoveChannelCommand>(c => c.YoutubeChannelId == youtubeChannelId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("NotFound", "Channel not found");
        _sender
            .Send(Arg.Any<RemoveChannelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        var youtubeChannelId = "UCtest123456789";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", youtubeChannelId } };
        _endpoint.SetHttpContext(httpContext);

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
