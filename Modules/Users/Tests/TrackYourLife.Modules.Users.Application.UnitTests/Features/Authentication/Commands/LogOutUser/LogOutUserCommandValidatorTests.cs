using FluentAssertions;
using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogOutUser;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.LogOutUser;

public class LogOutUserCommandValidatorTests
{
    private readonly LogOutUserCommandValidator _validator;

    public LogOutUserCommandValidatorTests()
    {
        _validator = new LogOutUserCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var command = new LogOutUserCommand(deviceId, false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyDeviceId_ShouldHaveValidationError()
    {
        // Arrange
        var deviceId = DeviceId.Empty;
        var command = new LogOutUserCommand(deviceId, false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeviceId);
    }

    [Fact]
    public void Validate_WithLogOutAllDevices_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var command = new LogOutUserCommand(deviceId, true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
