namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public sealed record GuitarTabSearchResultResponse(
    long? Id,
    string SongName,
    string ArtistName,
    string Type,
    int Votes,
    double? Rating,
    string? Tonality,
    int Version,
    string Part,
    string TabUrl,
    string? ArtistUrl
);

public sealed record GuitarTabSearchResponse(
    string Query,
    int ResultsCount,
    int Page,
    int TotalPages,
    IReadOnlyList<GuitarTabSearchResultResponse> Results
);
