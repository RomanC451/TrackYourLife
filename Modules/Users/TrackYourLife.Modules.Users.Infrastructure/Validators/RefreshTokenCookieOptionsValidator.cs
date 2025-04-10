using FluentValidation;
using TrackYourLife.Modules.Users.Infrastructure.Options;

namespace TrackYourLife.Modules.Users.Infrastructure.Validators;

internal sealed class RefreshTokenCookieOptionsValidator
    : AbstractValidator<RefreshTokenCookieOptions>
{
    public RefreshTokenCookieOptionsValidator()
    {
        RuleFor(x => x.DaysToExpire).GreaterThan(0);
        RuleFor(x => x.HttpOnly).NotNull();
        RuleFor(x => x.IsEssential).NotNull();
        RuleFor(x => x.Secure).NotNull();
        RuleFor(x => x.Domain).NotEmpty();
    }
}
