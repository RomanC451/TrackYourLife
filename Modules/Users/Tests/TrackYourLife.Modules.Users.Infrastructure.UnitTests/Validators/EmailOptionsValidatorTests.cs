using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Validators;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Validators;

public class EmailOptionsValidatorTests
{
    private readonly EmailOptionsValidator _validator;

    public EmailOptionsValidatorTests()
    {
        _validator = new EmailOptionsValidator();
    }

    [Fact]
    public void Validate_ValidOptions_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new EmailOptions
                {
                    SenderEmail = "test@example.com",
                    SmtpHost = "smtp.example.com",
                    SmtpPort = 587,
                    SmtpPassword = "test-password",
                    EmailTemplatePath = "templates/email.html",
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
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    public void Validate_InvalidSenderEmail_ShouldHaveValidationError(string? email)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new EmailOptions
                {
                    SenderEmail = email!,
                    SmtpHost = "smtp.example.com",
                    SmtpPort = 587,
                    SmtpPassword = "test-password",
                    EmailTemplatePath = "templates/email.html",
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SenderEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptySmtpHost_ShouldHaveValidationError(string? host)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new EmailOptions
                {
                    SenderEmail = "test@example.com",
                    SmtpHost = host!,
                    SmtpPort = 587,
                    SmtpPassword = "test-password",
                    EmailTemplatePath = "templates/email.html",
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SmtpHost);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void Validate_InvalidSmtpPort_ShouldHaveValidationError(int port)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new EmailOptions
                {
                    SenderEmail = "test@example.com",
                    SmtpHost = "smtp.example.com",
                    SmtpPort = port,
                    SmtpPassword = "test-password",
                    EmailTemplatePath = "templates/email.html",
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SmtpPort);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptySmtpPassword_ShouldHaveValidationError(string? password)
    {
        // Arrange
        var options = Microsoft
            .Extensions.Options.Options.Create(
                new EmailOptions
                {
                    SenderEmail = "test@example.com",
                    SmtpHost = "smtp.example.com",
                    SmtpPort = 587,
                    SmtpPassword = password!,
                    EmailTemplatePath = "templates/email.html",
                }
            )
            .Value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SmtpPassword);
    }
}
