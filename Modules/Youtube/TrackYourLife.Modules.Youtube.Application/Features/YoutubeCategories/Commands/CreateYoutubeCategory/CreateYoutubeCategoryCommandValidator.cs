using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.CreateYoutubeCategory;

internal sealed class CreateYoutubeCategoryCommandValidator : AbstractValidator<CreateYoutubeCategoryCommand>
{
    public CreateYoutubeCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(YoutubeCategory.MaxNameLength)
            .WithMessage("Category name is required.");

        RuleFor(x => x.MaxVideosPerDay)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Max videos per day must be 0 or greater.");
    }
}
