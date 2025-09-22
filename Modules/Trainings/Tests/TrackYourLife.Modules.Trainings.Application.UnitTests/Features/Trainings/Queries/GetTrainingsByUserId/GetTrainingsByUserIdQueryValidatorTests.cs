using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingsByUserId;

public class GetTrainingsByUserIdQueryValidatorTests
{
    private readonly GetTrainingsByUserIdQueryValidator _validator;

    public GetTrainingsByUserIdQueryValidatorTests()
    {
        _validator = new GetTrainingsByUserIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetTrainingsByUserIdQuery();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

