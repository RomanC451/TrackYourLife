using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;

internal sealed class UpdateYoutubeSettingsCommandValidator
    : AbstractValidator<UpdateYoutubeSettingsCommand>
{
    public UpdateYoutubeSettingsCommandValidator()
    {
        RuleFor(x => x.MaxEntertainmentVideosPerDay)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Max divertissment videos per day must be greater than or equal to 0.");

        RuleFor(x => x.SettingsChangeFrequency)
            .IsInEnum()
            .WithMessage("Invalid settings change frequency.");

        When(
            x => x.SettingsChangeFrequency == SettingsChangeFrequency.OnceEveryFewDays,
            () =>
            {
                RuleFor(x => x.DaysBetweenChanges)
                    .NotNull()
                    .WithMessage(
                        "Days between changes is required for 'OnceEveryFewDays' frequency."
                    )
                    .GreaterThan(0)
                    .WithMessage("Days between changes must be greater than 0.");

                RuleFor(x => x.SpecificDayOfWeek)
                    .Null()
                    .WithMessage(
                        "Specific day of week should not be set for 'OnceEveryFewDays' frequency."
                    );

                RuleFor(x => x.SpecificDayOfMonth)
                    .Null()
                    .WithMessage(
                        "Specific day of month should not be set for 'OnceEveryFewDays' frequency."
                    );
            }
        );

        When(
            x => x.SettingsChangeFrequency == SettingsChangeFrequency.SpecificDayOfWeek,
            () =>
            {
                RuleFor(x => x.SpecificDayOfWeek)
                    .NotNull()
                    .WithMessage(
                        "Specific day of week is required for 'SpecificDayOfWeek' frequency."
                    );

                RuleFor(x => x.DaysBetweenChanges)
                    .Null()
                    .WithMessage(
                        "Days between changes should not be set for 'SpecificDayOfWeek' frequency."
                    );

                RuleFor(x => x.SpecificDayOfMonth)
                    .Null()
                    .WithMessage(
                        "Specific day of month should not be set for 'SpecificDayOfWeek' frequency."
                    );
            }
        );

        When(
            x => x.SettingsChangeFrequency == SettingsChangeFrequency.SpecificDayOfMonth,
            () =>
            {
                RuleFor(x => x.SpecificDayOfMonth)
                    .NotNull()
                    .WithMessage(
                        "Specific day of month is required for 'SpecificDayOfMonth' frequency."
                    )
                    .InclusiveBetween(1, 31)
                    .WithMessage("Specific day of month must be between 1 and 31.");

                RuleFor(x => x.DaysBetweenChanges)
                    .Null()
                    .WithMessage(
                        "Days between changes should not be set for 'SpecificDayOfMonth' frequency."
                    );

                RuleFor(x => x.SpecificDayOfWeek)
                    .Null()
                    .WithMessage(
                        "Specific day of week should not be set for 'SpecificDayOfMonth' frequency."
                    );
            }
        );
    }
}
