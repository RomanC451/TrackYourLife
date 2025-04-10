using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryById;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Queries.GetFoodDiaryById;

public class GetFoodDiaryByIdQueryValidatorTests
{
    private readonly GetFoodDiaryByIdQueryValidator _validator;

    public GetFoodDiaryByIdQueryValidatorTests()
    {
        _validator = new GetFoodDiaryByIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveErrors()
    {
        // Arrange
        var query = new GetFoodDiaryByIdQuery(NutritionDiaryId.NewId());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError()
    {
        // Arrange
        var query = new GetFoodDiaryByIdQuery(NutritionDiaryId.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DiaryId);
    }
}
