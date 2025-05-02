using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.LogInUser;

public class LogInUserCommandHandlerTests
{
    private readonly IUserQuery _userQuery;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthService _authService;
    private readonly LogInUserCommandHandler _handler;

    public LogInUserCommandHandlerTests()
    {
        _userQuery = Substitute.For<IUserQuery>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _authService = Substitute.For<IAuthService>();
        _handler = new LogInUserCommandHandler(_userQuery, _passwordHasher, _authService);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Pasdasassword123!";
        var deviceId = DeviceId.NewId();
        var command = new LogInUserCommand(email, password, deviceId);

        var user = UserFaker.GenerateReadModel(verifiedOnUtc: DateTime.UtcNow);

        _userQuery.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(user);

        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        var jwtToken = "test-jwt-token";
        var refreshToken = TokenFaker.Generate();
        _authService
            .RefreshUserAuthTokensAsync(user, deviceId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success((jwtToken, refreshToken))));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(Error.None);
        result.IsSuccess.Should().BeTrue();
        result.Value.Item1.Should().Be(jwtToken);
        result.Value.Item2.Should().Be(refreshToken);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ReturnsFailure()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Pasdasassword123!";
        var deviceId = DeviceId.NewId();
        var command = new LogInUserCommand(email, password, deviceId);

        _userQuery
            .GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithUnverifiedEmail_ReturnsFailure()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Pasdasassword123!";
        var deviceId = DeviceId.NewId();
        var command = new LogInUserCommand(email, password, deviceId);

        var user = UserFaker.GenerateReadModel(verifiedOnUtc: null);

        _userQuery.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(user);

        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.NotVerified);
    }
}
