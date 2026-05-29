using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.VerifyYoutubeSettingsPassword;

internal sealed class VerifyYoutubeSettingsPasswordCommandValidator
    : AbstractValidator<VerifyYoutubeSettingsPasswordCommand>
{
    public VerifyYoutubeSettingsPasswordCommandValidator()
    {
        RuleFor(x => x.Password).NotEmpty();
    }
}
