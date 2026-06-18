using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Services;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Application.UnitTests.Services;

public class ReadingSessionNotesGrouperTests
{
    [Fact]
    public void GroupByChapter_OrdersGroupsAndNotesByNewestFirst()
    {
        var older = new DateTime(2026, 6, 10, 10, 0, 0, DateTimeKind.Utc);
        var middle = new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var newest = new DateTime(2026, 6, 15, 18, 0, 0, DateTimeKind.Utc);

        var notes = new[]
        {
            CreateNote("Cap. 4 — Focus", "First note", older),
            CreateNote("cap. 4 — focus", "Second note", middle),
            CreateNote("Cap. 3 — Distractions", "Newest chapter note", newest),
        };

        var groups = ReadingSessionNotesGrouper.GroupByChapter(notes);

        groups.Should().HaveCount(2);
        groups[0].ChapterTitle.Should().Be("Cap. 3 — Distractions");
        groups[0].Notes[0].Content.Should().Be("Newest chapter note");
        groups[1].ChapterTitle.Should().Be("cap. 4 — focus");
        groups[1].Notes[0].Content.Should().Be("Second note");
        groups[1].Notes[1].Content.Should().Be("First note");
    }

    private static ReadingSessionNoteReadModel CreateNote(
        string chapterTitle,
        string content,
        DateTime createdOnUtc
    ) =>
        new(
            ReadingSessionNoteId.NewId(),
            ReadingSessionId.NewId(),
            BookId.NewId(),
            UserId.NewId(),
            chapterTitle,
            content,
            createdOnUtc,
            DateOnly.FromDateTime(createdOnUtc)
        );
}
