using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.VerifyEmail;

public class VerifyEmailCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly VerifyEmailCommandHandler _handler;

    public VerifyEmailCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _tokenRepository = Substitute.For<ITokenRepository>();
        _handler = new VerifyEmailCommandHandler(_userRepository, _tokenRepository);
    }

    [Fact]
    public async Task Handle_WithValidToken_ReturnsSuccess()
    {
        // Arrange
        var token = "valid-token-12345678901234567890123456789012";
        var command = new VerifyEmailCommand(token);
        var userId = UserId.NewId();
        var user = UserFaker.Generate();
        var emailVerificationToken = Token
            .Create(
                TokenId.NewId(),
                token,
                userId,
                TokenType.EmailVerificationToken,
                DateTime.UtcNow.AddHours(1)
            )
            .Value;

        _tokenRepository
            .GetByValueAsync(token, Arg.Any<CancellationToken>())
            .Returns(emailVerificationToken);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
        _tokenRepository.Received(1).Remove(emailVerificationToken);
    }

    [Fact]
    public async Task Handle_WithInvalidToken_ReturnsFailure()
    {
        // Arrange
        var token = "invalid-token";
        var command = new VerifyEmailCommand(token);

        _tokenRepository.GetByValueAsync(token, Arg.Any<CancellationToken>()).Returns((Token?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailVerificationToken.Invalid);
        await _userRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
        _tokenRepository.DidNotReceive().Remove(Arg.Any<Token>());
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ReturnsFailure()
    {
        // Arrange
        var token = "valid-token-12345678901234567890123456789012";
        var command = new VerifyEmailCommand(token);
        var userId = UserId.NewId();
        var emailVerificationToken = Token
            .Create(
                TokenId.NewId(),
                token,
                userId,
                TokenType.EmailVerificationToken,
                DateTime.UtcNow.AddHours(1)
            )
            .Value;

        _tokenRepository
            .GetByValueAsync(token, Arg.Any<CancellationToken>())
            .Returns(emailVerificationToken);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotFound(userId));
        _tokenRepository.DidNotReceive().Remove(Arg.Any<Token>());
    }
}
