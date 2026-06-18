using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Services;

internal static class ReadingSessionNotesGrouper
{
    public static IReadOnlyList<BookChapterNotesGroupDto> GroupByChapter(
        IReadOnlyList<ReadingSessionNoteReadModel> notes
    )
    {
        return notes
            .GroupBy(note => note.ChapterTitle, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var orderedNotes = group.OrderByDescending(note => note.CreatedOnUtc).ToList();

                return new
                {
                    Group = new BookChapterNotesGroupDto(
                        orderedNotes[0].ChapterTitle,
                        null,
                        orderedNotes
                            .Select(note => new BookChapterNoteEntryDto(
                                note.Id.Value,
                                note.ReadingSessionId.Value,
                                note.SessionDate ?? DateOnly.FromDateTime(note.CreatedOnUtc),
                                note.Content,
                                note.CreatedOnUtc
                            ))
                            .ToList()
                    ),
                    LatestCreatedOnUtc = orderedNotes[0].CreatedOnUtc,
                };
            })
            .OrderByDescending(entry => entry.LatestCreatedOnUtc)
            .Select(entry => entry.Group)
            .ToList();
    }
}
