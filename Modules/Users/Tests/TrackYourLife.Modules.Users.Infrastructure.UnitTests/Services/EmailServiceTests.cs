using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NSubstitute;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Services;

public class EmailServiceTests : IDisposable
{
    private readonly IOptions<EmailOptions> _emailOptions;
    private readonly ISmtpClient _smtpClient;
    private readonly EmailService _emailService;
    private readonly string _testEmail;
    private readonly string _testVerificationLink;
    private readonly string _testTemplatePath;
    private bool _disposed;

    public EmailServiceTests()
    {
        _testTemplatePath = Path.Combine(Path.GetTempPath(), "test-email-template.html");
        _emailOptions = Microsoft.Extensions.Options.Options.Create(
            new EmailOptions
            {
                SenderEmail = "test@example.com",
                SmtpHost = "smtp.example.com",
                SmtpPort = 587,
                SmtpPassword = "test-password",
                EmailTemplatePath = _testTemplatePath,
            }
        );

        _smtpClient = Substitute.For<ISmtpClient>();
        _emailService = new EmailService(_emailOptions, _smtpClient);
        _testEmail = "user@example.com";
        _testVerificationLink = "https://test.com/verify-email?token=test-token";
    }

    [Fact]
    public void SendVerificationEmail_ValidEmailAndLink_ShouldReturnSuccess()
    {
        // Arrange
        var testEmailTemplate =
            @"
            <html>
                <body>
                    <p>Please verify your email by clicking the link below:</p>
                    <a href=""[verification-link]"">Verify Email</a>
                </body>
            </html>";

        File.WriteAllText(_testTemplatePath, testEmailTemplate);

        // Act
        var result = _emailService.SendVerificationEmail(_testEmail, _testVerificationLink);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _smtpClient
            .Received(1)
            .Connect(
                _emailOptions.Value.SmtpHost,
                _emailOptions.Value.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls
            );
        _smtpClient
            .Received(1)
            .Authenticate(_emailOptions.Value.SenderEmail, _emailOptions.Value.SmtpPassword);
        _smtpClient.Received(1).Send(Arg.Any<MimeMessage>());
        _smtpClient.Received(1).Disconnect(true);
    }

    [Fact]
    public void SendVerificationEmail_InvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var invalidEmail = "invalid-email";
        var testEmailTemplate =
            @"
            <html>
                <body>
                    <p>Please verify your email by clicking the link below:</p>
                    <a href=""[verification-link]"">Verify Email</a>
                </body>
            </html>";

        File.WriteAllText(_testTemplatePath, testEmailTemplate);

        _smtpClient
            .When(x =>
                x.Connect(
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<MailKit.Security.SecureSocketOptions>()
                )
            )
            .Do(x => throw new Exception());

        // Act
        var result = _emailService.SendVerificationEmail(invalidEmail, _testVerificationLink);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.EmailService.FailedToSendEmail);
    }

    [Fact]
    public void SendVerificationEmail_InvalidSmtpSettings_ShouldReturnFailure()
    {
        // Arrange
        var invalidEmailOptions = Microsoft.Extensions.Options.Options.Create(
            new EmailOptions
            {
                SenderEmail = "test@example.com",
                SmtpHost = "invalid-host",
                SmtpPort = 587,
                SmtpPassword = "test-password",
                EmailTemplatePath = _testTemplatePath,
            }
        );

        var emailService = new EmailService(invalidEmailOptions, _smtpClient);
        var testEmailTemplate =
            @"
            <html>
                <body>
                    <p>Please verify your email by clicking the link below:</p>
                    <a href=""[verification-link]"">Verify Email</a>
                </body>
            </html>";

        File.WriteAllText(_testTemplatePath, testEmailTemplate);

        _smtpClient
            .When(x =>
                x.Connect(
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<MailKit.Security.SecureSocketOptions>()
                )
            )
            .Do(x => throw new Exception());

        // Act
        var result = emailService.SendVerificationEmail(_testEmail, _testVerificationLink);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.EmailService.FailedToSendEmail);
    }

    [Fact]
    public void SendVerificationEmail_MissingTemplateFile_ShouldReturnFailure()
    {
        // Arrange
        if (File.Exists(_testTemplatePath))
        {
            File.Delete(_testTemplatePath);
        }

        // Act
        var result = _emailService.SendVerificationEmail(_testEmail, _testVerificationLink);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.EmailService.EmailTemplateNotFound);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing && File.Exists(_testTemplatePath))
        {
            File.Delete(_testTemplatePath);
        }

        _disposed = true;
    }
}
