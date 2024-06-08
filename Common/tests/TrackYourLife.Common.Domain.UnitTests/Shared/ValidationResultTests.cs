using TrackYourLife.Common.Domain.Shared;
using Xunit;

namespace TrackYourLife.Common.Domain.UnitTests.Shared;

public class ValidationResultTests
{
    [Fact]
    public void WithErrors_ReturnsValidationResultInstance_WhenErrorsIsNotNull()
    {
        // Arrange
        Error[] errors = new Error[]
        {
            new Error("Code1", "Message1"),
            new Error("Code2", "Message2")
        };

        // Act
        ValidationResult result = ValidationResult.WithErrors(errors);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(IValidationResult.ValidationError, result.Error);
        Assert.Equal(errors, result.Errors);
    }
}
