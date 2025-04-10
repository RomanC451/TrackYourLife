using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Foods.Queries.GetFoodById;

public class GetFoodByIdQueryValidatorTests
{
    private readonly GetFoodByIdQueryValidator _validator;

    public GetFoodByIdQueryValidatorTests()
    {
        _validator = new GetFoodByIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenFoodIdIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetFoodByIdQuery(null!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FoodId);
    }

    [Fact]
    public void Validate_WhenFoodIdIsNotNull_ShouldBeValid()
    {
        // Arrange
        var query = new GetFoodByIdQuery(FoodId.NewId());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
