using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Validators;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Validators;

public class ClientAppOptionsValidatorTests
{
    private readonly ClientAppOptionsValidator _validator;

    public ClientAppOptionsValidatorTests()
    {
        _validator = new ClientAppOptionsValidator();
    }

    [Fact]
    public void Validate_ValidOptions_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new ClientAppOptions
                {
                    BaseUrl = "https://example.com",
                    EmailVerificationPath = "verify-email",
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
    [InlineData("invalid-url")]
    [InlineData("/example.com")]
    [InlineData("example.com/")]
    public void Validate_InvalidBaseUrl_ShouldHaveValidationError(string baseUrl)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new ClientAppOptions { BaseUrl = baseUrl, EmailVerificationPath = "verify-email" }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BaseUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData("/verify-email")]
    [InlineData("verify-email/")]
    public void Validate_InvalidEmailVerificationPath_ShouldHaveValidationError(string path)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new ClientAppOptions
                {
                    BaseUrl = "https://example.com",
                    EmailVerificationPath = path,
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailVerificationPath);
    }
}
