using FluentValidation;
using TrackYourLife.Modules.Users.Infrastructure.Options;

namespace TrackYourLife.Modules.Users.Infrastructure.Validators;

public class ClientAppOptionsValidator : AbstractValidator<ClientAppOptions>
{
    public ClientAppOptionsValidator()
    {
        RuleFor(x => x.BaseUrl)
            .NotEmpty()
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Invalid URL format")
            .Must(x => !x.StartsWith('/') && !x.EndsWith('/'))
            .WithMessage("BaseUrl should not start or end with '/'");

        RuleFor(x => x.EmailVerificationPath)
            .NotEmpty()
            .Must(x => !x.StartsWith('/') && !x.EndsWith('/'))
            .WithMessage("EmailVerificationPath should not start or end with '/'");
    }
}
