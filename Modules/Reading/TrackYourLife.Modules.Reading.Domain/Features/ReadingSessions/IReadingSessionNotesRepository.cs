namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public interface IReadingSessionNotesRepository
{
    Task AddAsync(ReadingSessionNote note, CancellationToken cancellationToken = default);
}
