using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;

    public EmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public Result SendVerificationEmail(Email userEmail, string verificationLink)
    {
        string htmlFilePath = "./wwwroot/email.html";
        string htmlContent = File.ReadAllText(htmlFilePath);

        htmlContent = htmlContent.Replace("[verification-link]", verificationLink);

        var email = new MimeMessage();
        email.To.Add(MailboxAddress.Parse(userEmail.Value));
        email.Subject = "TrackYourLife - Verify your emai address";

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };

        email.Body = bodyBuilder.ToMessageBody();

        var result = SendEmail(email);

        return result;
    }

    private Result SendEmail(MimeMessage email)
    {
        email.From.Add(MailboxAddress.Parse(_emailOptions.SenderEmail));

        try
        {
            using var smtp = new SmtpClient();
            smtp.Connect(
                _emailOptions.SmtpHost,
                _emailOptions.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls
            );
            smtp.Authenticate(_emailOptions.SenderEmail, _emailOptions.SmtpPassword);

            smtp.Send(email);
            smtp.Disconnect(true);
        }
        catch
        {
            return Result.Failure(InfrastructureErrors.EmailService.FailedToSendEmail);
        }

        return Result.Success();
    }
}
