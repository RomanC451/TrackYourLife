using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Commands;

public class AddChannelToCategoryTests
{
    private readonly ISender _sender;
    private readonly AddChannelToCategory _endpoint;

    public AddChannelToCategoryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new AddChannelToCategory(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        var channelId = YoutubeChannelId.NewId();
        var catGuid = Guid.NewGuid();
        _sender
            .Send(Arg.Any<AddChannelToCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(channelId));

        var request = new AddChannelToCategoryRequest(YoutubeChannelId: "UCtest123", YoutubeCategoryId: catGuid);

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value!.Id.Should().Be(channelId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<AddChannelToCategoryCommand>(c =>
                    c.YoutubeChannelId == "UCtest123" && c.YoutubeCategoryId == YoutubeCategoryId.Create(catGuid)
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        var error = new Error("ConflictError", "Channel already exists");
        _sender
            .Send(Arg.Any<AddChannelToCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubeChannelId>(error));

        var request = new AddChannelToCategoryRequest("UCtest123", Guid.NewGuid());

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
