using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

public static class SavedGuitarTabErrors
{
    public static readonly Func<Guid, Error> NotFound = tabId =>
        new("SavedGuitarTabs.NotFound", $"Saved tab with id '{tabId}' was not found.", 404);

    public static readonly Error InvalidYoutubeUrl =
        new("SavedGuitarTabs.InvalidYoutubeUrl", "Enter a YouTube video link.", 400);
}
