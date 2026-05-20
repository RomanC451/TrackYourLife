using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryLimit;

internal sealed class UpdateYoutubeCategoryLimitCommandValidator
    : AbstractValidator<UpdateYoutubeCategoryLimitCommand>
{
    public UpdateYoutubeCategoryLimitCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEqual(YoutubeCategoryId.Empty)
            .WithMessage("Category id is required.");

        RuleFor(x => x.MaxVideosPerDay)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Max videos per day must be 0 or greater.");
    }
}
