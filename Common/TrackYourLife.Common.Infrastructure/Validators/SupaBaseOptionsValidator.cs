using FluentValidation;
using TrackYourLife.Common.Infrastructure.Options;

namespace TrackYourLife.Common.Infrastructure.Validators;

public sealed class SupaBaseOptionsValidator : AbstractValidator<SupaBaseOptions>
{
    public SupaBaseOptionsValidator()
    {
        RuleFor(x => x.Url).NotEmpty().Must(BeAValidUri).WithMessage("The Url is not a valid URI.");
        RuleFor(x => x.Key).NotEmpty();
    }

    private bool BeAValidUri(string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out _);
    }
}
