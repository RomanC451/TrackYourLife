using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;

internal sealed class AddChannelToCategoryCommandValidator : AbstractValidator<AddChannelToCategoryCommand>
{
    public AddChannelToCategoryCommandValidator()
    {
        RuleFor(x => x.YoutubeChannelId)
            .NotEmpty()
            .WithMessage("YouTube channel ID is required.");

        RuleFor(x => x.YoutubeCategoryId).NotEqual(YoutubeCategoryId.Empty).WithMessage("Category is required.");
    }
}
