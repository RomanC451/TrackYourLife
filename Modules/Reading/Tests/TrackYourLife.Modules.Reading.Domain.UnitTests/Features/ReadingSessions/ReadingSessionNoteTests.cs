using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.UnitTests.Features.ReadingSessions;

public class ReadingSessionNoteTests
{
    private static readonly ReadingSessionNoteId TestNoteId = ReadingSessionNoteId.NewId();
    private static readonly ReadingSessionId TestSessionId = ReadingSessionId.NewId();
    private static readonly BookId TestBookId = BookId.NewId();
    private static readonly UserId TestUserId = UserId.NewId();

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = ReadingSessionNote.Create(
            TestNoteId,
            TestSessionId,
            TestBookId,
            TestUserId,
            "Cap. 1 — Start",
            "Important idea about focus.",
            DateTime.UtcNow
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.ChapterTitle.Should().Be("Cap. 1 — Start");
        result.Value.Content.Should().Be("Important idea about focus.");
    }

    [Fact]
    public void Create_WithEmptyChapterTitle_ShouldFail()
    {
        var result = ReadingSessionNote.Create(
            TestNoteId,
            TestSessionId,
            TestBookId,
            TestUserId,
            "   ",
            "Content",
            DateTime.UtcNow
        );

        result.IsFailure.Should().BeTrue();
    }
}
