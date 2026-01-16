using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Commands.RemoveChannel;

public sealed class RemoveChannelCommandHandlerTests
{
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly RemoveChannelCommandHandler _handler;

    public RemoveChannelCommandHandlerTests()
    {
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _handler = new RemoveChannelCommandHandler(_youtubeChannelsRepository);
    }

    [Fact]
    public async Task Handle_WhenChannelNotFound_ReturnsFailure()
    {
        // Arrange
        var youtubeChannelId = "UCtest123456789";
        var command = new RemoveChannelCommand(youtubeChannelId);

        _youtubeChannelsRepository
            .GetByYoutubeChannelIdAsync(youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns((YoutubeChannel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeChannelsErrors.NotFound(youtubeChannelId));
        _youtubeChannelsRepository.DidNotReceive().Remove(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenChannelExists_RemovesChannel()
    {
        // Arrange
        var youtubeChannelId = "UCtest123456789";
        var channelId = YoutubeChannelId.NewId();
        var command = new RemoveChannelCommand(youtubeChannelId);
        var channel = YoutubeChannel
            .Create(
                channelId,
                UserId.NewId(),
                youtubeChannelId,
                "Channel Name",
                "thumbnail-url",
                VideoCategory.Entertainment,
                DateTime.UtcNow
            )
            .Value;

        _youtubeChannelsRepository
            .GetByYoutubeChannelIdAsync(youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _youtubeChannelsRepository.Received(1).Remove(channel);
    }
}
