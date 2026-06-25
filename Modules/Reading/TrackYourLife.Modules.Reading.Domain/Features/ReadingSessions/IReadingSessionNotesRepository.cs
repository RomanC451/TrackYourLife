namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public interface IReadingSessionNotesRepository
{
    Task<ReadingSessionNote?> GetByIdAsync(
        ReadingSessionNoteId id,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(ReadingSessionNote note, CancellationToken cancellationToken = default);

    void Update(ReadingSessionNote note);

    void Remove(ReadingSessionNote note);
}
