using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.SetChannelFavorite;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Commands;

public class SetChannelFavoriteTests
{
    private readonly ISender _sender;
    private readonly SetChannelFavorite _endpoint;

    public SetChannelFavoriteTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new SetChannelFavorite(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        _sender
            .Send(Arg.Any<SetChannelFavoriteCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        var youtubeChannelId = "UCtest123456789";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", youtubeChannelId } };
        _endpoint.SetHttpContext(httpContext);

        var result = await _endpoint.ExecuteAsync(
            new SetChannelFavoriteRequest(true),
            CancellationToken.None
        );

        result.Should().BeOfType<NoContent>();
        await _sender
            .Received(1)
            .Send(
                Arg.Is<SetChannelFavoriteCommand>(c =>
                    c.YoutubeChannelId == youtubeChannelId && c.IsFavorite),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        var error = new Error("NotFound", "Channel not found");
        _sender
            .Send(Arg.Any<SetChannelFavoriteCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", "UCmissing" } };
        _endpoint.SetHttpContext(httpContext);

        var result = await _endpoint.ExecuteAsync(
            new SetChannelFavoriteRequest(false),
            CancellationToken.None
        );

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
