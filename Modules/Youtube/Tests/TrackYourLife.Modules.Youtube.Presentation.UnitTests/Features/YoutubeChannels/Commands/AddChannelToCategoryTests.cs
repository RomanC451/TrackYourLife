using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;
using TrackYourLife.Modules.Youtube.Domain.Core;
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
        // Arrange
        var channelId = YoutubeChannelId.NewId();
        _sender
            .Send(Arg.Any<AddChannelToCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(channelId));

        var request = new AddChannelToCategoryRequest(
            YoutubeChannelId: "UCtest123",
            Category: VideoCategory.Entertainment
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(channelId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<AddChannelToCategoryCommand>(c =>
                    c.YoutubeChannelId == "UCtest123" && c.Category == VideoCategory.Entertainment
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("ConflictError", "Channel already exists");
        _sender
            .Send(Arg.Any<AddChannelToCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubeChannelId>(error));

        var request = new AddChannelToCategoryRequest(
            YoutubeChannelId: "UCtest123",
            Category: VideoCategory.Educational
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapRequestToCommandCorrectly()
    {
        // Arrange
        var channelId = YoutubeChannelId.NewId();
        _sender
            .Send(Arg.Any<AddChannelToCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(channelId));

        var request = new AddChannelToCategoryRequest(
            YoutubeChannelId: "UCtest456",
            Category: VideoCategory.Educational
        );

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<AddChannelToCategoryCommand>(c =>
                    c.YoutubeChannelId == "UCtest456" && c.Category == VideoCategory.Educational
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
