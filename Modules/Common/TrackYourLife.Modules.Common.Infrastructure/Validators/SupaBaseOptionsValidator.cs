using FluentValidation;
using TrackYourLife.Modules.Common.Infrastructure.Options;

namespace TrackYourLife.Modules.Common.Infrastructure.Validators;

internal sealed class SupaBaseOptionsValidator : AbstractValidator<SupaBaseOptions>
{
    public SupaBaseOptionsValidator()
    {
        RuleFor(x => x.Url).NotEmpty().Must(BeAValidUri).WithMessage("The Url is not a valid URI.");
        RuleFor(x => x.Key).NotEmpty();
    }

    private static bool BeAValidUri(string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out _);
    }
}
