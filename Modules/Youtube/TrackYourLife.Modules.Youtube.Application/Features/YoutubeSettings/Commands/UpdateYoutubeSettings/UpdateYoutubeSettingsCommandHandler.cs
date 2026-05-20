using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;

internal sealed class UpdateYoutubeSettingsCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeSettingsRepository youtubeSettingsRepository,
    IYoutubeCategoriesRepository youtubeCategoriesRepository,
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

        var categories = await youtubeCategoriesRepository.ListByUserIdOrderedAsync(
            userId,
            cancellationToken
        );

        if (categories.Count == 0)
        {
            return Result.Failure<YoutubeSettingsId>(YoutubeCategoriesErrors.NotFoundForUser(userId));
        }

        var existingSettings = await youtubeSettingsRepository.GetByUserIdAsync(userId, cancellationToken);

        YoutubeSetting settings;

        if (existingSettings is null)
        {
            var settingsId = YoutubeSettingsId.NewId();
            var createResult = YoutubeSetting.Create(
                id: settingsId,
                userId: userId,
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
            var canChangeResult = existingSettings.CanChangeSettings(utcNow);

            if (canChangeResult.IsFailure)
            {
                return Result.Failure<YoutubeSettingsId>(canChangeResult.Error);
            }

            var updateResult = existingSettings.UpdateSettings(
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
