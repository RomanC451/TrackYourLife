using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.SetChannelFavorite;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Commands.SetChannelFavorite;

public sealed class SetChannelFavoriteCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SetChannelFavoriteCommandHandler _handler;

    public SetChannelFavoriteCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new SetChannelFavoriteCommandHandler(
            _userIdentifierProvider,
            _youtubeChannelsRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenChannelNotFound_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var command = new SetChannelFavoriteCommand("UCmissing", true);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, command.YoutubeChannelId, Arg.Any<CancellationToken>())
            .Returns((YoutubeChannel?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _youtubeChannelsRepository.DidNotReceive().Update(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenFavoriteAlreadyMatches_ReturnsSuccessWithoutUpdate()
    {
        var userId = UserId.NewId();
        var channel = CreateChannel(userId, isFavorite: true);
        var command = new SetChannelFavoriteCommand(channel.YoutubeChannelId, true);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, command.YoutubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _youtubeChannelsRepository.DidNotReceive().Update(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenFavoriteChanges_UpdatesChannel()
    {
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var channel = CreateChannel(userId, isFavorite: false);
        var command = new SetChannelFavoriteCommand(channel.YoutubeChannelId, true);

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, command.YoutubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        channel.IsFavorite.Should().BeTrue();
        channel.ModifiedOnUtc.Should().Be(utcNow);
        _youtubeChannelsRepository.Received(1).Update(channel);
    }

    [Fact]
    public async Task Handle_WhenUnfavoriting_UpdatesChannel()
    {
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var channel = CreateChannel(userId, isFavorite: true);
        var command = new SetChannelFavoriteCommand(channel.YoutubeChannelId, false);

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, command.YoutubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        channel.IsFavorite.Should().BeFalse();
        channel.ModifiedOnUtc.Should().Be(utcNow);
        _youtubeChannelsRepository.Received(1).Update(channel);
    }

    private static YoutubeChannel CreateChannel(UserId userId, bool isFavorite)
    {
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                "UCtest123",
                "Test Channel",
                null,
                YoutubeCategoryId.NewId(),
                "Cat",
                DateTime.UtcNow
            )
            .Value;

        if (isFavorite)
        {
            channel.SetFavorite(true, DateTime.UtcNow);
        }

        return channel;
    }
}
