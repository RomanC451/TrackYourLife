using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.SetYoutubeSettingsPassword;

internal sealed class SetYoutubeSettingsPasswordCommandValidator
    : AbstractValidator<SetYoutubeSettingsPasswordCommand>
{
    public SetYoutubeSettingsPasswordCommandValidator()
    {
        When(
            x => !string.IsNullOrEmpty(x.NewPassword),
            () =>
            {
                RuleFor(x => x.NewPassword!)
                    .Must(password => YoutubeSettingsPassword.Create(password).IsSuccess)
                    .WithMessage(
                        "New password must be at least 10 characters and include upper, lower, number, and special character."
                    );
            }
        );
    }
}
