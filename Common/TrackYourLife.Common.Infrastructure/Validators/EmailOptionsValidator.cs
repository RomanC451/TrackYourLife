using FluentValidation;
using TrackYourLife.Common.Infrastructure.Options;

namespace TrackYourLife.Common.Infrastructure.Validators;

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
