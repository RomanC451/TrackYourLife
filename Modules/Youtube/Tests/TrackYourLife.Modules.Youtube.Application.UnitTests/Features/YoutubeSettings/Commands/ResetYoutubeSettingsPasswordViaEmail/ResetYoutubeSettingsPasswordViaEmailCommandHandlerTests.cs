using MassTransit;
using MassTransit.Testing;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.ResetYoutubeSettingsPasswordViaEmail;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeSettings.Commands.ResetYoutubeSettingsPasswordViaEmail;

public sealed class ResetYoutubeSettingsPasswordViaEmailCommandHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeSettingsRepository _youtubeSettingsRepository;
    private readonly IYoutubeSettingsPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private ResetYoutubeSettingsPasswordViaEmailCommandHandler _handler = null!;

    private GetUserAccountByIdResponse _accountResponse = null!;
    private SendYoutubeSettingsPasswordResetEmailResponse _emailResponse = null!;
    private readonly DateTime _utcNow = new(2026, 5, 21, 12, 0, 0, DateTimeKind.Utc);

    public ResetYoutubeSettingsPasswordViaEmailCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeSettingsRepository = Substitute.For<IYoutubeSettingsRepository>();
        _passwordHasher = Substitute.For<IYoutubeSettingsPasswordHasher>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _dateTimeProvider.UtcNow.Returns(_utcNow);
    }

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();
        _accountResponse = new GetUserAccountByIdResponse(
            new UserAccountDto("user@example.com", _utcNow.AddDays(-1)),
            []
        );
        _emailResponse = new SendYoutubeSettingsPasswordResetEmailResponse([]);

        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "account",
                e =>
                {
                    e.Handler<GetUserAccountByIdRequest>(async context =>
                    {
                        await context.RespondAsync(_accountResponse);
                    });
                }
            );
            configurator.ReceiveEndpoint(
                "email",
                e =>
                {
                    e.Handler<SendYoutubeSettingsPasswordResetEmailRequest>(async context =>
                    {
                        await context.RespondAsync(_emailResponse);
                    });
                }
            );
        };

        await _harness.Start();
        _bus = _harness.Bus;
        _handler = new ResetYoutubeSettingsPasswordViaEmailCommandHandler(
            _userIdentifierProvider,
            _youtubeSettingsRepository,
            _passwordHasher,
            _dateTimeProvider,
            _bus
        );
    }

    public async Task DisposeAsync() => await _harness.Stop();

    [Fact]
    public async Task Handle_WhenPasswordNotSet_ShouldFail()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((YoutubeSetting?)null);

        var result = await _handler.Handle(
            new ResetYoutubeSettingsPasswordViaEmailCommand(),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.PasswordNotSet);
    }

    [Fact]
    public async Task Handle_WhenRateLimited_ShouldFail()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        var settings = YoutubeSetting.Create(
            YoutubeSettingsId.NewId(),
            userId,
            "old-hash",
            _utcNow
        ).Value;
        settings.RecordPasswordResetEmailSent(_utcNow.AddMinutes(-2));

        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);

        var result = await _handler.Handle(
            new ResetYoutubeSettingsPasswordViaEmailCommand(),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.ResetEmailRateLimited);
    }

    [Fact]
    public async Task Handle_WhenEmailNotVerified_ShouldFail()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        var settings = YoutubeSetting
            .Create(YoutubeSettingsId.NewId(), userId, "old-hash", _utcNow)
            .Value;

        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);
        _accountResponse = new GetUserAccountByIdResponse(
            new UserAccountDto("user@example.com", null),
            []
        );

        var result = await _handler.Handle(
            new ResetYoutubeSettingsPasswordViaEmailCommand(),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.AccountEmailNotVerified);
        settings.SettingsPasswordHash.Should().Be("old-hash");
    }

    [Fact]
    public async Task Handle_WhenEmailFails_ShouldRestorePreviousHash()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        var settings = YoutubeSetting
            .Create(YoutubeSettingsId.NewId(), userId, "old-hash", _utcNow)
            .Value;

        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);
        _accountResponse = new GetUserAccountByIdResponse(
            new UserAccountDto("user@example.com", _utcNow.AddDays(-1)),
            []
        );
        _emailResponse = new SendYoutubeSettingsPasswordResetEmailResponse(
            [YoutubeSettingsErrors.FailedToSendResetEmail]
        );
        _passwordHasher.Hash(Arg.Any<string>()).Returns("new-hash");

        var result = await _handler.Handle(
            new ResetYoutubeSettingsPasswordViaEmailCommand(),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.FailedToSendResetEmail);
        settings.SettingsPasswordHash.Should().Be("old-hash");
        _youtubeSettingsRepository.Received(2).Update(settings);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldUpdateHashAndRecordSentTime()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        var settings = YoutubeSetting
            .Create(YoutubeSettingsId.NewId(), userId, "old-hash", _utcNow)
            .Value;

        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);
        _accountResponse = new GetUserAccountByIdResponse(
            new UserAccountDto("user@example.com", _utcNow.AddDays(-1)),
            []
        );
        _emailResponse = new SendYoutubeSettingsPasswordResetEmailResponse([]);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("new-hash");

        var result = await _handler.Handle(
            new ResetYoutubeSettingsPasswordViaEmailCommand(),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        settings.SettingsPasswordHash.Should().Be("new-hash");
        settings.SettingsPasswordResetEmailSentAtUtc.Should().Be(_utcNow);
        _youtubeSettingsRepository.Received(2).Update(settings);
    }
}
