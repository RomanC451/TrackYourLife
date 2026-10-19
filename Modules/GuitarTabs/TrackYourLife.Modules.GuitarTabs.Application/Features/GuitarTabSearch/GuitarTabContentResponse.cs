namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public sealed record GuitarTabChordResponse(
    string Name,
    IReadOnlyList<int> Frets,
    int Position,
    int? BarreFret,
    int? BarreFromString,
    int? BarreToString
);

public sealed record GuitarTabContentResponse(
    long Id,
    string SongName,
    string ArtistName,
    string Type,
    string? Tonality,
    string? TuningName,
    string? Tuning,
    int? Capo,
    double? Rating,
    int Votes,
    string Content,
    string TabUrl,
    IReadOnlyList<GuitarTabChordResponse> Chords,
    string? YoutubeUrl
);
