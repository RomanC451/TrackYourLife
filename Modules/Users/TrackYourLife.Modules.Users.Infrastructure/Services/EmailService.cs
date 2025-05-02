using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

internal sealed class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ISmtpClient _smtpClient;

    public EmailService(IOptions<EmailOptions> emailOptions, ISmtpClient smtpClient)
    {
        _emailOptions = emailOptions.Value;
        _smtpClient = smtpClient;
    }

    public Result SendVerificationEmail(string userEmail, string verificationLink)
    {
        if (!File.Exists(_emailOptions.EmailTemplatePath))
        {
            return Result.Failure(InfrastructureErrors.EmailService.EmailTemplateNotFound);
        }

        string htmlContent = File.ReadAllText(_emailOptions.EmailTemplatePath);
        htmlContent = htmlContent.Replace("[verification-link]", verificationLink);

        var email = new MimeMessage();
        email.To.Add(MailboxAddress.Parse(userEmail));
        email.Subject = "TrackYourLife - Verify your email address";

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };
        email.Body = bodyBuilder.ToMessageBody();

        return SendEmail(email);
    }

    private Result SendEmail(MimeMessage email)
    {
        email.From.Add(MailboxAddress.Parse(_emailOptions.SenderEmail));

        try
        {
            _smtpClient.Connect(
                _emailOptions.SmtpHost,
                _emailOptions.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls
            );
            _smtpClient.Authenticate(_emailOptions.SenderEmail, _emailOptions.SmtpPassword);
            _smtpClient.Send(email);
            _smtpClient.Disconnect(true);
        }
        catch
        {
            return Result.Failure(InfrastructureErrors.EmailService.FailedToSendEmail);
        }

        return Result.Success();
    }
}
