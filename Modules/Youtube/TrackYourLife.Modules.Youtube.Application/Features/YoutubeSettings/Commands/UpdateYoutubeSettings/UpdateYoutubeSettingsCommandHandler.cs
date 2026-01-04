using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;

internal sealed class UpdateYoutubeSettingsCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeSettingsRepository youtubeSettingsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdateYoutubeSettingsCommand, YoutubeSettingsId>
{
    public async Task<Result<YoutubeSettingsId>> Handle(
        UpdateYoutubeSettingsCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var utcNow = dateTimeProvider.UtcNow;

        // Get existing settings or create new
        var existingSettings = await youtubeSettingsRepository.GetByUserIdAsync(
            userId,
            cancellationToken
        );

        Domain.Features.YoutubeSettings.YoutubeSetting settings;

        if (existingSettings is null)
        {
            // Create new settings
            var settingsId = YoutubeSettingsId.NewId();
            var createResult = Domain.Features.YoutubeSettings.YoutubeSetting.Create(
                id: settingsId,
                userId: userId,
                maxEntertainmentVideosPerDay: request.MaxEntertainmentVideosPerDay,
                settingsChangeFrequency: request.SettingsChangeFrequency,
                daysBetweenChanges: request.DaysBetweenChanges,
                lastSettingsChangeUtc: utcNow,
                specificDayOfWeek: request.SpecificDayOfWeek,
                specificDayOfMonth: request.SpecificDayOfMonth,
                createdOnUtc: utcNow
            );

            if (createResult.IsFailure)
            {
                return Result.Failure<YoutubeSettingsId>(createResult.Error);
            }

            settings = createResult.Value;
            await youtubeSettingsRepository.AddAsync(settings, cancellationToken);
        }
        else
        {
            // Check if settings can be changed
            var canChangeResult = existingSettings.CanChangeSettings(utcNow);

            if (canChangeResult.IsFailure)
            {
                return Result.Failure<YoutubeSettingsId>(canChangeResult.Error);
            }

            // Update existing settings
            var updateResult = existingSettings.UpdateSettings(
                maxEntertainmentVideosPerDay: request.MaxEntertainmentVideosPerDay,
                settingsChangeFrequency: request.SettingsChangeFrequency,
                daysBetweenChanges: request.DaysBetweenChanges,
                specificDayOfWeek: request.SpecificDayOfWeek,
                specificDayOfMonth: request.SpecificDayOfMonth,
                utcNow: utcNow
            );

            if (updateResult.IsFailure)
            {
                return Result.Failure<YoutubeSettingsId>(updateResult.Error);
            }

            settings = existingSettings;
            youtubeSettingsRepository.Update(settings);
        }

        return Result.Success(settings.Id);
    }
}
