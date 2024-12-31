using TrackYourLife.Modules.Common.Infrastructure.Options;
using TrackYourLife.Modules.Common.Infrastructure.Validators;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Validators;

public class SupaBaseOptionsValidatorTest : IDisposable
{
    private readonly SupaBaseOptionsValidator sut;

    public SupaBaseOptionsValidatorTest()
    {
        sut = new SupaBaseOptionsValidator();
    }

    [Fact]
    public void Validate_WhenOptionsAreValid_ShouldNotHaveError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = "https://supabase.io", Key = "key" };

        // Act
        var result = sut.Validate(options);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenUrlIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = string.Empty };

        // Act
        var result = sut.Validate(options);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(e =>
                e.PropertyName == "Url" && e.ErrorMessage == "The Url is not a valid URI."
            );
    }

    [Fact]
    public void Validate_WhenUrlIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = "invalid-url" };

        // Act
        var result = sut.Validate(options);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(e =>
                e.PropertyName == "Url" && e.ErrorMessage == "The Url is not a valid URI."
            );
    }

    [Fact]
    public void Validate_WhenKeyIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Key = string.Empty };

        // Act
        var result = sut.Validate(options);

        // Assert
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Key");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // No mocks to clear in this case
    }
}
