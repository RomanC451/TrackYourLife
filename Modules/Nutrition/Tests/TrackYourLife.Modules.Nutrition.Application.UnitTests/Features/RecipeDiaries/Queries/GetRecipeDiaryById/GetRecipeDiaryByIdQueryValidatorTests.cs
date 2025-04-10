using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Queries.GetRecipeDiaryById;

public class GetRecipeDiaryByIdQueryValidatorTests
{
    private readonly GetRecipeDiaryByIdQueryValidator _validator;

    public GetRecipeDiaryByIdQueryValidatorTests()
    {
        _validator = new GetRecipeDiaryByIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new GetRecipeDiaryByIdQuery(NutritionDiaryId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new GetRecipeDiaryByIdQuery(NutritionDiaryId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DiaryId);
    }
}
