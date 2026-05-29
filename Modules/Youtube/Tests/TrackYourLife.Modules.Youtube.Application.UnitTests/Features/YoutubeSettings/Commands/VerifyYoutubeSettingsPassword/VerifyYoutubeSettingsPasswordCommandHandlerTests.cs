using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.VerifyYoutubeSettingsPassword;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeSettings.Commands.VerifyYoutubeSettingsPassword;

public sealed class VerifyYoutubeSettingsPasswordCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeSettingsRepository _youtubeSettingsRepository;
    private readonly IYoutubeSettingsPasswordHasher _passwordHasher;
    private readonly VerifyYoutubeSettingsPasswordCommandHandler _handler;

    public VerifyYoutubeSettingsPasswordCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeSettingsRepository = Substitute.For<IYoutubeSettingsRepository>();
        _passwordHasher = Substitute.For<IYoutubeSettingsPasswordHasher>();
        _handler = new VerifyYoutubeSettingsPasswordCommandHandler(
            _userIdentifierProvider,
            _youtubeSettingsRepository,
            _passwordHasher
        );
    }

    [Fact]
    public async Task Handle_WhenPasswordCorrect_ShouldSucceed()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);

        var settings = YoutubeSetting
            .Create(YoutubeSettingsId.NewId(), userId, "stored-hash", DateTime.UtcNow)
            .Value;

        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);

        _passwordHasher.Verify("stored-hash", "correct").Returns(true);

        var result = await _handler.Handle(
            new VerifyYoutubeSettingsPasswordCommand("correct"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenPasswordIncorrect_ShouldFail()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);

        var settings = YoutubeSetting
            .Create(YoutubeSettingsId.NewId(), userId, "stored-hash", DateTime.UtcNow)
            .Value;

        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);

        _passwordHasher.Verify("stored-hash", "wrong").Returns(false);

        var result = await _handler.Handle(
            new VerifyYoutubeSettingsPasswordCommand("wrong"),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.InvalidPassword);
    }
}
