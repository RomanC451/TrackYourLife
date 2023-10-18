
using FluentValidation;
using TrackYourLifeDotnet.Infrastructure.Options;

namespace TrackYourLifeDotnet.Infrastructure.Validators;

public class JwtOptionsValidator : AbstractValidator<JwtOptions>
{
    public JwtOptionsValidator()
    {
        RuleFor(x => x.Issuer).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Audience).NotEmpty().MaximumLength(100);

        RuleFor(x => x.SecretKey).NotEmpty().MaximumLength(50);

        RuleFor(x => x.MinutesToExpire).NotEmpty().GreaterThan(0);
    }
}
