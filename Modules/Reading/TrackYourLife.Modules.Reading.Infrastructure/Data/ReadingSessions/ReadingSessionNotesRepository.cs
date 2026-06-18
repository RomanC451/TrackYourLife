using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.ReadingSessions;

internal sealed class ReadingSessionNotesRepository(ReadingWriteDbContext context)
    : IReadingSessionNotesRepository
{
    public Task AddAsync(ReadingSessionNote note, CancellationToken cancellationToken = default) =>
        context.ReadingSessionNotes.AddAsync(note, cancellationToken).AsTask();
}
