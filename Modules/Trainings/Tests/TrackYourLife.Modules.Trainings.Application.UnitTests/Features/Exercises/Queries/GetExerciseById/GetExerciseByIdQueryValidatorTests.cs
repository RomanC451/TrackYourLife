using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseById;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Queries.GetExerciseById;

public class GetExerciseByIdQueryValidatorTests
{
    private readonly GetExerciseByIdQueryValidator _validator;

    public GetExerciseByIdQueryValidatorTests()
    {
        _validator = new GetExerciseByIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetExerciseByIdQuery(ExerciseId.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetExerciseByIdQuery(ExerciseId.NewId());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

