using FluentValidation;
using TrackYourLife.Modules.Youtube.Application.Options;

namespace TrackYourLife.Modules.Youtube.Application.Validators;

internal sealed class YoutubeModuleOptionsValidator : AbstractValidator<YoutubeModuleOptions>
{
    public YoutubeModuleOptionsValidator()
    {
        RuleFor(x => x.MaxCategoriesForPro).InclusiveBetween(2, 500);
    }
}
