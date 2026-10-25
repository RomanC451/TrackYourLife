namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;

public sealed record GuitarTabChordShapeResponse(
    IReadOnlyList<int> Frets,
    IReadOnlyList<int> Fingers,
    int Position,
    int? BarreFret,
    int? BarreFromString,
    int? BarreToString,
    int? BarreFinger
);

public sealed record GuitarTabChordResponse(
    string Name,
    IReadOnlyList<GuitarTabChordShapeResponse> Shapes
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
