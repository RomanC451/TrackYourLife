using FluentValidation;
using TrackYourLifeDotnet.Infrastructure.Options;

namespace TrackYourLifeDotnet.Infrastructure.Validators;

public class FoodApiOptionsValidator : AbstractValidator<FoodApiOptions>
{
    public FoodApiOptionsValidator()
    {
        RuleFor(x => x.BaseUrl)
            .NotEmpty()
            .Must(
                url =>
                    Uri.TryCreate(url, UriKind.Absolute, out Uri? outUri)
                    && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)
            );

        RuleFor(x => x.BaseApiUrl)
            .NotEmpty()
            .Must(
                url =>
                    Uri.TryCreate(url, UriKind.Absolute, out Uri? outUri)
                    && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps)
            );

        RuleFor(x => x.SearchPath).NotEmpty();

        RuleFor(x => x.LogInFormPath).NotEmpty();

        RuleFor(x => x.LogInJsonPath).NotEmpty();

        RuleFor(x => x.AuthTokenPath).NotEmpty();

        RuleFor(x => x.CookieDoamins).NotEmpty();

        RuleFor(x => x.SpaceEncoded).NotEmpty();

        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
