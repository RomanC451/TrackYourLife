using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public static class GuitarTabSearchErrors
{
    public static readonly Error Unavailable =
        new(
            "GuitarTabSearch.Unavailable",
            "Ultimate Guitar search is unavailable.",
            502
        );

    public static readonly Error InvalidResponse =
        new(
            "GuitarTabSearch.InvalidResponse",
            "Ultimate Guitar search returned an unexpected response.",
            502
        );

    public static readonly Error InvalidTabUrl =
        new(
            "GuitarTabSearch.InvalidTabUrl",
            "Only a public Ultimate Guitar tab page can be opened.",
            400
        );

    public static readonly Error ContentUnavailable =
        new(
            "GuitarTabSearch.ContentUnavailable",
            "This tab is not available to view.",
            404
        );
}
