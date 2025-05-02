using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.DeleteCurrentUser;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.DeleteCurrentUser;

public sealed class RemoveCurrentUserCommandValidatorTests
{
    private readonly RemoveCurrentUserCommandValidator _validator;

    public RemoveCurrentUserCommandValidatorTests()
    {
        _validator = new RemoveCurrentUserCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new RemoveCurrentUserCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
