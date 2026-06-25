namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record ReadingPagesDataPointDto(
    DateOnly Date,
    int Pages,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null
);
