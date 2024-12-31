using FluentValidation;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Commands.UpdateFoodApiCookies;

public sealed class UpdateFoodApiCookiesCommandValidator
    : AbstractValidator<UpdateFoodApiCookiesCommand>
{
    public UpdateFoodApiCookiesCommandValidator()
    {
        RuleFor(x => x.CookieFile).NotNull().WithMessage("Cookie file is required.");
    }
}
