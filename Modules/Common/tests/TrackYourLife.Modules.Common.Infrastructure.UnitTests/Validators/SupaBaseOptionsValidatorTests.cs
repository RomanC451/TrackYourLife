using FluentValidation.TestHelper;
using TrackYourLife.Modules.Common.Infrastructure.Options;
using TrackYourLife.Modules.Common.Infrastructure.Validators;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Validators;

public class SupaBaseOptionsValidatorTests
{
    private readonly SupaBaseOptionsValidator _sut;

    public SupaBaseOptionsValidatorTests()
    {
        _sut = new SupaBaseOptionsValidator();
    }

    [Fact]
    public void Validate_WhenOptionsAreValid_ShouldNotHaveError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = "https://supabase.io", Key = "test-key" };

        // Act
        var result = _sut.TestValidate(options);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUrlIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = string.Empty, Key = "test-key" };

        // Act
        var result = _sut.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Url);
    }

    [Fact]
    public void Validate_WhenUrlIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = null!, Key = "test-key" };

        // Act
        var result = _sut.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Url);
    }

    [Fact]
    public void Validate_WhenUrlIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = "invalid-url", Key = "test-key" };

        // Act
        var result = _sut.TestValidate(options);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Url)
            .WithErrorMessage("The Url is not a valid URI.");
    }

    [Fact]
    public void Validate_WhenKeyIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = "https://supabase.io", Key = string.Empty };

        // Act
        var result = _sut.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Fact]
    public void Validate_WhenKeyIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var options = new SupaBaseOptions { Url = "https://supabase.io", Key = null! };

        // Act
        var result = _sut.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Theory]
    [InlineData("https://supabase.io")]
    [InlineData("http://supabase.io")]
    [InlineData("https://supabase.io:8080")]
    [InlineData("https://sub.supabase.io")]
    public void Validate_WhenUrlIsValid_ShouldNotHaveValidationError(string url)
    {
        // Arrange
        var options = new SupaBaseOptions { Url = url, Key = "test-key" };

        // Act
        var result = _sut.TestValidate(options);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Url);
    }
}
