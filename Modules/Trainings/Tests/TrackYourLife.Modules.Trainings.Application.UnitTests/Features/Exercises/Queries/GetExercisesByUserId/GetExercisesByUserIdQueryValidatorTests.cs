using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExercisesByUserId;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Queries.GetExercisesByUserId;

public class GetExercisesByUserIdQueryValidatorTests
{
    private readonly GetExercisesByUserIdQueryValidator _validator;

    public GetExercisesByUserIdQueryValidatorTests()
    {
        _validator = new GetExercisesByUserIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetExercisesByUserIdQuery();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

