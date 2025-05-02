using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.ResendVerificationEmail;

public class ResendEmailVerificationCommandHandlerTests
{
    private readonly IUserQuery _userQuery;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ResendEmailVerificationCommandHandler _handler;

    public ResendEmailVerificationCommandHandlerTests()
    {
        _userQuery = Substitute.For<IUserQuery>();
        _authService = Substitute.For<IAuthService>();
        _emailService = Substitute.For<IEmailService>();
        _handler = new ResendEmailVerificationCommandHandler(
            _userQuery,
            _authService,
            _emailService
        );
    }

    [Fact]
    public async Task Handle_WithValidEmail_ReturnsSuccess()
    {
        // Arrange
        var email = "test@example.com";
        var command = new ResendEmailVerificationCommand(email);
        var user = UserFaker.GenerateReadModel(verifiedOnUtc: null);
        var verificationLink = "https://example.com/verify-email?token=test-token";

        _userQuery.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(user);

        _authService
            .GenerateEmailVerificationLinkAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Success(verificationLink));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userQuery
            .Received(1)
            .GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>());
        await _authService
            .Received(1)
            .GenerateEmailVerificationLinkAsync(user.Id, Arg.Any<CancellationToken>());
        _emailService.Received(1).SendVerificationEmail(user.Email, verificationLink);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ReturnsFailure()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var command = new ResendEmailVerificationCommand(email);

        _userQuery
            .GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.EmailNotFound);
        await _authService
            .DidNotReceive()
            .GenerateEmailVerificationLinkAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
        _emailService.DidNotReceive().SendVerificationEmail(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WithAlreadyVerifiedUser_ReturnsFailure()
    {
        // Arrange
        var email = "verified@example.com";
        var command = new ResendEmailVerificationCommand(email);
        var user = UserFaker.GenerateReadModel(verifiedOnUtc: DateTime.UtcNow);

        _userQuery.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.AlreadyVerified);
        await _authService
            .DidNotReceive()
            .GenerateEmailVerificationLinkAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
        _emailService.DidNotReceive().SendVerificationEmail(Arg.Any<string>(), Arg.Any<string>());
    }
}
