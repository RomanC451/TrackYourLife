using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.SetChannelFavorite;

internal sealed class SetChannelFavoriteCommandValidator : AbstractValidator<SetChannelFavoriteCommand>
{
    public SetChannelFavoriteCommandValidator()
    {
        RuleFor(x => x.YoutubeChannelId).NotEmpty();
    }
}
