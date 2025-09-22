using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

public class GetOngoingTrainingByUserIdQueryValidatorTests
{
    private readonly GetOngoingTrainingByUserIdQueryValidator _validator;

    public GetOngoingTrainingByUserIdQueryValidatorTests()
    {
        _validator = new GetOngoingTrainingByUserIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetOngoingTrainingByUserIdQuery();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

