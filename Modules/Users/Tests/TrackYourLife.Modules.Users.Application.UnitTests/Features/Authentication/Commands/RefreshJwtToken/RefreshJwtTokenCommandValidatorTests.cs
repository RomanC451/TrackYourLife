using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.RefreshJwtToken;

public class RefreshJwtTokenCommandValidatorTests
{
    private readonly RefreshJwtTokenCommandValidator _validator;

    public RefreshJwtTokenCommandValidatorTests()
    {
        _validator = new RefreshJwtTokenCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var command = new RefreshJwtTokenCommand("valid-refresh-token", deviceId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyRefreshToken_ShouldHaveValidationError()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var command = new RefreshJwtTokenCommand(string.Empty, deviceId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RefreshTokenValue);
    }

    [Fact]
    public void Validate_WithEmptyDeviceId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new RefreshJwtTokenCommand("valid-refresh-token", DeviceId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceId);
    }
}
