using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.SetYoutubeSettingsPassword;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeSettings.Commands.SetYoutubeSettingsPassword;

public sealed class SetYoutubeSettingsPasswordCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeSettingsRepository _youtubeSettingsRepository;
    private readonly IYoutubeSettingsPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SetYoutubeSettingsPasswordCommandHandler _handler;
    private readonly DateTime _utcNow = new(2026, 5, 21, 12, 0, 0, DateTimeKind.Utc);

    public SetYoutubeSettingsPasswordCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeSettingsRepository = Substitute.For<IYoutubeSettingsRepository>();
        _passwordHasher = Substitute.For<IYoutubeSettingsPasswordHasher>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _dateTimeProvider.UtcNow.Returns(_utcNow);
        _handler = new SetYoutubeSettingsPasswordCommandHandler(
            _userIdentifierProvider,
            _youtubeSettingsRepository,
            _passwordHasher,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenSettingInitialPassword_ShouldAddSettings()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((YoutubeSetting?)null);
        _passwordHasher.Hash("ValidPass1!").Returns("new-hash");

        var result = await _handler.Handle(
            new SetYoutubeSettingsPasswordCommand(null, "ValidPass1!"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        await _youtubeSettingsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubeSetting>(s => s.SettingsPasswordHash == "new-hash"),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenRemovingPassword_WithWrongCurrent_ShouldFail()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);

        var settings = YoutubeSetting
            .Create(YoutubeSettingsId.NewId(), userId, "stored-hash", _utcNow)
            .Value;

        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);
        _passwordHasher.Verify("stored-hash", "wrong").Returns(false);

        var result = await _handler.Handle(
            new SetYoutubeSettingsPasswordCommand("wrong", null),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.InvalidPassword);
    }
}
