using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Users.Events;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.DomainEvents;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Events;

public sealed class UserRegisteredDomainEventHandlerTests
{
    private readonly IUserQuery _userQuery;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;
    private readonly UsersFeatureManagement _featureManager;
    private readonly UserRegisteredDomainEventHandler _handler;

    public UserRegisteredDomainEventHandlerTests()
    {
        _userQuery = Substitute.For<IUserQuery>();
        _emailService = Substitute.For<IEmailService>();
        _authService = Substitute.For<IAuthService>();
        _featureManager = Substitute.For<UsersFeatureManagement>();
        _handler = new UserRegisteredDomainEventHandler(
            _userQuery,
            _emailService,
            _authService,
            _featureManager
        );
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_DoesNotSendEmail()
    {
        // Arrange
        var userId = UserId.NewId();
        var @event = new UserRegisteredDomainEvent(userId);

        _userQuery.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((UserReadModel?)null);

        // Act
        await _handler.Handle(@event, CancellationToken.None);

        // Assert
        await _authService
            .DidNotReceive()
            .GenerateEmailVerificationLinkAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
        _emailService.DidNotReceive().SendVerificationEmail(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenEmailVerificationSkipped_DoesNotSendEmail()
    {
        // Arrange
        var userId = UserId.NewId();
        var user = UserFaker.GenerateReadModel(id: userId);
        var @event = new UserRegisteredDomainEvent(userId);

        _userQuery.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _featureManager.SkipEmailVerification = true;

        // Act
        await _handler.Handle(@event, CancellationToken.None);

        // Assert
        await _authService
            .DidNotReceive()
            .GenerateEmailVerificationLinkAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
        _emailService.DidNotReceive().SendVerificationEmail(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenLinkGenerationFails_DoesNotSendEmail()
    {
        // Arrange
        var userId = UserId.NewId();
        var user = UserFaker.GenerateReadModel(id: userId);
        var @event = new UserRegisteredDomainEvent(userId);

        _userQuery.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _featureManager.SkipEmailVerification = false;
        _authService
            .GenerateEmailVerificationLinkAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string>(InfrastructureErrors.SupaBaseClient.ClientNotWorking));

        // Act
        await _handler.Handle(@event, CancellationToken.None);

        // Assert
        await _authService
            .Received(1)
            .GenerateEmailVerificationLinkAsync(userId, Arg.Any<CancellationToken>());
        _emailService.DidNotReceive().SendVerificationEmail(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenAllConditionsMet_SendsVerificationEmail()
    {
        // Arrange
        var userId = UserId.NewId();
        var user = UserFaker.GenerateReadModel(id: userId);
        var @event = new UserRegisteredDomainEvent(userId);
        var verificationLink = "https://example.com/verify?token=123";

        _userQuery.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _featureManager.SkipEmailVerification = false;
        _authService
            .GenerateEmailVerificationLinkAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(verificationLink));

        // Act
        await _handler.Handle(@event, CancellationToken.None);

        // Assert
        await _authService
            .Received(1)
            .GenerateEmailVerificationLinkAsync(userId, Arg.Any<CancellationToken>());
        _emailService.Received(1).SendVerificationEmail(user.Email, verificationLink);
    }
}
