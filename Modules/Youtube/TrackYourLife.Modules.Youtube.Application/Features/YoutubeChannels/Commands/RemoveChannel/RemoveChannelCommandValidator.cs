using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;

internal sealed class RemoveChannelCommandValidator : AbstractValidator<RemoveChannelCommand>
{
    public RemoveChannelCommandValidator()
    {
        RuleFor(x => x.YoutubeChannelId).NotEmpty().WithMessage("Channel ID is required.");
    }
}
