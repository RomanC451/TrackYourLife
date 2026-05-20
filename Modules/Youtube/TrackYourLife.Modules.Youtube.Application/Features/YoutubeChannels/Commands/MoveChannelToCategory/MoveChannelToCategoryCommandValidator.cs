using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.MoveChannelToCategory;

internal sealed class MoveChannelToCategoryCommandValidator
    : AbstractValidator<MoveChannelToCategoryCommand>
{
    public MoveChannelToCategoryCommandValidator()
    {
        RuleFor(x => x.YoutubeChannelId)
            .NotEmpty()
            .WithMessage("YouTube channel ID is required.");

        RuleFor(x => x.TargetYoutubeCategoryId)
            .NotEqual(YoutubeCategoryId.Empty)
            .WithMessage("Target category is required.");
    }
}
