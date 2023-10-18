using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Infrastructure.Options;

namespace TrackYourLifeDotnet.Infrastructure.Services;

public sealed class EmailService : IEmailService
{

    private readonly EmailOptions _emailOptions;

    public EmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public Task SendVerifitionEmail(Email userEmail, string verificationLink){
        

        string htmlFilePath = "./wwwroot/email.html";
        string htmlContent = File.ReadAllText(htmlFilePath);
        
        htmlContent = htmlContent.Replace("[verification-link]", verificationLink);

        var email = new MimeMessage();
        email.To.Add(MailboxAddress.Parse(userEmail.Value));
        email.Subject = "TrackYourLife - Verify your emai address";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlContent
        };

        email.Body = bodyBuilder.ToMessageBody();




        SendEmail(email);

        return Task.CompletedTask;
    }

    private Task SendEmail(MimeMessage email){

        email.From.Add(MailboxAddress.Parse(_emailOptions.SenderEmail));

        using var smtp = new SmtpClient();
        smtp.Connect(_emailOptions.SmtpHost, _emailOptions.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        smtp.Authenticate(_emailOptions.SenderEmail, _emailOptions.SmtpPassword);

        smtp.Send(email);
        smtp.Disconnect(true);

        return Task.CompletedTask;
    }
    
}