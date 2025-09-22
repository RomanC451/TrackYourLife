using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

public class GetOngoingTrainingByIdQueryValidatorTests
{
    private readonly GetOngoingTrainingByIdQueryValidator _validator;

    public GetOngoingTrainingByIdQueryValidatorTests()
    {
        _validator = new GetOngoingTrainingByIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetOngoingTrainingByIdQuery(OngoingTrainingId.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetOngoingTrainingByIdQuery(OngoingTrainingId.NewId());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

