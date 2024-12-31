using FluentValidation;
using TrackYourLife.Modules.Users.Infrastructure.Options;

namespace TrackYourLife.Modules.Users.Infrastructure.Validators;

public class EmailOptionsValidator : AbstractValidator<EmailOptions>
{
    public EmailOptionsValidator()
    {
        RuleFor(x => x.SenderEmail).NotEmpty().EmailAddress();

        RuleFor(x => x.SmtpHost).NotEmpty();

        RuleFor(x => x.SmtpPort).NotEmpty().GreaterThan(0).LessThan(65536);

        RuleFor(x => x.SmtpPassword).NotEmpty();
    }
}
