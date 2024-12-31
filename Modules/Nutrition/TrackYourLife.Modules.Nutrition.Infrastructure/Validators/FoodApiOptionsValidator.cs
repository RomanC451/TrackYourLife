using FluentValidation;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Validators;

/// <summary>
/// Validates the options for the Food API.
/// </summary>
public sealed class FoodApiOptionsValidator : AbstractValidator<FoodApiOptions>
{
    public FoodApiOptionsValidator()
    {
        RuleFor(x => x.BaseUrl)
            .NotEmpty()
            .Must(url =>
                Uri.TryCreate(url, UriKind.Absolute, out Uri? outUri)
                && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)
            );

        RuleFor(x => x.BaseApiUrl)
            .NotEmpty()
            .Must(url =>
                Uri.TryCreate(url, UriKind.Absolute, out Uri? outUri)
                && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)
            );

        RuleFor(x => x.SearchPath).NotEmpty();

        RuleFor(x => x.AuthTokenPath).NotEmpty();

        RuleFor(x => x.CookieDomains).NotEmpty();

        RuleFor(x => x.SpaceEncoded).NotEmpty();
    }
}
