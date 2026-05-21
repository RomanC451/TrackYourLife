using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Commands.RemoveChannel;

public sealed class RemoveChannelCommandHandlerTests
{
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly RemoveChannelCommandHandler _handler;

    public RemoveChannelCommandHandlerTests()
    {
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new RemoveChannelCommandHandler(_userIdentifierProvider, _youtubeChannelsRepository);
    }

    [Fact]
    public async Task Handle_WhenChannelNotFound_ReturnsFailure()
    {
        var youtubeChannelId = "UCtest123456789";
        var command = new RemoveChannelCommand(youtubeChannelId);

        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns((YoutubeChannel?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeChannelsErrors.NotFound(youtubeChannelId));
        _youtubeChannelsRepository.DidNotReceive().Remove(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenChannelOwnedByAnotherUser_ReturnsFailure()
    {
        var youtubeChannelId = "UCtest123456789";
        var command = new RemoveChannelCommand(youtubeChannelId);
        var currentUserId = UserId.NewId();

        _userIdentifierProvider.UserId.Returns(currentUserId);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(
                currentUserId,
                youtubeChannelId,
                Arg.Any<CancellationToken>()
            )
            .Returns((YoutubeChannel?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeChannelsErrors.NotFound(youtubeChannelId));
        _youtubeChannelsRepository.DidNotReceive().Remove(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenChannelExistsForCurrentUser_RemovesChannel()
    {
        var youtubeChannelId = "UCtest123456789";
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var command = new RemoveChannelCommand(youtubeChannelId);
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                youtubeChannelId,
                "Channel Name",
                "thumbnail-url",
                categoryId,
                "Cat",
                DateTime.UtcNow
            )
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _youtubeChannelsRepository.Received(1).Remove(channel);
    }
}
