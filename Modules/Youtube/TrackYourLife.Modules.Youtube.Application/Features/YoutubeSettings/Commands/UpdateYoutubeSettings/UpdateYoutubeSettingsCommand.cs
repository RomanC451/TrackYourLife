using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;

public sealed record UpdateYoutubeSettingsCommand(
    int MaxEntertainmentVideosPerDay,
    SettingsChangeFrequency SettingsChangeFrequency,
    int? DaysBetweenChanges,
    DayOfWeek? SpecificDayOfWeek,
    int? SpecificDayOfMonth
) : ICommand<YoutubeSettingsId>;
