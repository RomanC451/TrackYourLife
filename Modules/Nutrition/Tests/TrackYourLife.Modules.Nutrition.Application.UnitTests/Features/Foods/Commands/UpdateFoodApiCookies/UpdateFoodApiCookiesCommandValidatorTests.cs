using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Commands.UpdateFoodApiCookies;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Foods.Commands.UpdateFoodApiCookies;

public class UpdateFoodApiCookiesCommandValidatorTests
{
    private readonly UpdateFoodApiCookiesCommandValidator _validator;

    public UpdateFoodApiCookiesCommandValidatorTests()
    {
        _validator = new UpdateFoodApiCookiesCommandValidator();
    }

    [Fact]
    public void Validate_WhenCookieFileIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateFoodApiCookiesCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CookieFile);
    }

    [Fact]
    public void Validate_WhenCookieFileIsNotNull_ShouldBeValid()
    {
        // Arrange
        var formFile = Substitute.For<IFormFile>();
        var command = new UpdateFoodApiCookiesCommand(formFile);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
