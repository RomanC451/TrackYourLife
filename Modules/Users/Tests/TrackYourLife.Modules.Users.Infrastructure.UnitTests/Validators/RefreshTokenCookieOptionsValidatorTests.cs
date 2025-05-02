using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Validators;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Validators;

public class RefreshTokenCookieOptionsValidatorTests
{
    private readonly RefreshTokenCookieOptionsValidator _validator;

    public RefreshTokenCookieOptionsValidatorTests()
    {
        _validator = new RefreshTokenCookieOptionsValidator();
    }

    [Fact]
    public void Validate_ValidOptions_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new RefreshTokenCookieOptions
                {
                    DaysToExpire = 7,
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    Domain = "example.com",
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidDaysToExpire_ShouldHaveValidationError(int days)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new RefreshTokenCookieOptions
                {
                    DaysToExpire = days,
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    Domain = "example.com",
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DaysToExpire);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyDomain_ShouldHaveValidationError(string? domain)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new RefreshTokenCookieOptions
                {
                    DaysToExpire = 7,
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    Domain = domain!,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Domain);
    }
}
