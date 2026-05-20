using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.MoveChannelToCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Commands;

public class MoveChannelToCategoryTests
{
    private readonly ISender _sender;
    private readonly MoveChannelToCategory _endpoint;

    public MoveChannelToCategoryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new MoveChannelToCategory(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        var youtubeChannelId = "UCtest123456789";
        var targetCategoryId = YoutubeCategoryId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", youtubeChannelId } };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<MoveChannelToCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new MoveChannelToCategoryRequest(targetCategoryId.Value);

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<MoveChannelToCategoryCommand>(c =>
                    c.YoutubeChannelId == youtubeChannelId
                    && c.TargetYoutubeCategoryId == targetCategoryId
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        var youtubeChannelId = "UCtest123456789";
        var targetCategoryId = YoutubeCategoryId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary { { "id", youtubeChannelId } };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Channel not found");
        _sender
            .Send(Arg.Any<MoveChannelToCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new MoveChannelToCategoryRequest(targetCategoryId.Value);

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
