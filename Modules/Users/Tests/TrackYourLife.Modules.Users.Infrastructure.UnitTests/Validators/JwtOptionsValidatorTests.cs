using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Validators;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Validators;

public class JwtOptionsValidatorTests
{
    private readonly JwtOptionsValidator _validator;

    public JwtOptionsValidatorTests()
    {
        _validator = new JwtOptionsValidator();
    }

    [Fact]
    public void Validate_ValidOptions_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = "test-issuer",
                    Audience = "test-audience",
                    SecretKey = new string('a', 100),
                    MinutesToExpire = 60,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidIssuer_ShouldHaveValidationError(string? issuer)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = issuer!,
                    Audience = "test-audience",
                    SecretKey = new string('a', 100),
                    MinutesToExpire = 60,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Issuer);
    }

    [Fact]
    public void Validate_IssuerTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = new string('a', 101),
                    Audience = "test-audience",
                    SecretKey = new string('a', 100),
                    MinutesToExpire = 60,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Issuer);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidAudience_ShouldHaveValidationError(string? audience)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = "test-issuer",
                    Audience = audience!,
                    SecretKey = new string('a', 100),
                    MinutesToExpire = 60,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Audience);
    }

    [Fact]
    public void Validate_AudienceTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = "test-issuer",
                    Audience = new string('a', 101),
                    SecretKey = new string('a', 100),
                    MinutesToExpire = 60,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Audience);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("a")]
    public void Validate_InvalidSecretKey_ShouldHaveValidationError(string? secretKey)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = "test-issuer",
                    Audience = "test-audience",
                    SecretKey = secretKey!,
                    MinutesToExpire = 60,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SecretKey);
    }

    [Fact]
    public void Validate_SecretKeyTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = "test-issuer",
                    Audience = "test-audience",
                    SecretKey = new string('a', 99),
                    MinutesToExpire = 60,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SecretKey);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidMinutesToExpire_ShouldHaveValidationError(int minutes)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new JwtOptions
                {
                    Issuer = "test-issuer",
                    Audience = "test-audience",
                    SecretKey = new string('a', 100),
                    MinutesToExpire = minutes,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinutesToExpire);
    }
}
