using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.UnitTests.Features.ReadingSessions;

public class ReadingSessionCreateFinishedTests
{
    [Fact]
    public void CreateFinished_WithValidPages_CreatesFinishedSession()
    {
        var now = DateTime.UtcNow;

        var result = ReadingSession.CreateFinished(
            ReadingSessionId.NewId(),
            UserId.NewId(),
            BookId.NewId(),
            startPage: 100,
            endPage: 352,
            totalPages: 400,
            DateOnly.FromDateTime(now),
            now,
            now
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.IsActive.Should().BeFalse();
        result.Value.StartPage.Should().Be(100);
        result.Value.EndPage.Should().Be(352);
        result.Value.PagesRead.Should().Be(252);
        result.Value.FinishedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public void CreateFinished_WhenEndPageExceedsTotal_Fails()
    {
        var now = DateTime.UtcNow;

        var result = ReadingSession.CreateFinished(
            ReadingSessionId.NewId(),
            UserId.NewId(),
            BookId.NewId(),
            startPage: 100,
            endPage: 500,
            totalPages: 400,
            DateOnly.FromDateTime(now),
            now,
            now
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReadingSessionErrors.EndPageExceedsTotalPages);
    }
}
