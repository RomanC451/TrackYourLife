using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.ReadingSessions;

internal sealed class ReadingSessionNotesRepository(ReadingWriteDbContext context)
    : IReadingSessionNotesRepository
{
    public Task<ReadingSessionNote?> GetByIdAsync(
        ReadingSessionNoteId id,
        CancellationToken cancellationToken = default
    ) => context.ReadingSessionNotes.FirstOrDefaultAsync(note => note.Id == id, cancellationToken);

    public Task AddAsync(ReadingSessionNote note, CancellationToken cancellationToken = default) =>
        context.ReadingSessionNotes.AddAsync(note, cancellationToken).AsTask();

    public void Update(ReadingSessionNote note) => context.ReadingSessionNotes.Update(note);

    public void Remove(ReadingSessionNote note) => context.ReadingSessionNotes.Remove(note);
}
