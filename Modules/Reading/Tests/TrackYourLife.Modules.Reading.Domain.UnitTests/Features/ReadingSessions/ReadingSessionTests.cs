using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.UnitTests.Features.ReadingSessions;

public class ReadingSessionTests
{
    private static readonly ReadingSessionId TestSessionId = ReadingSessionId.NewId();
    private static readonly UserId TestUserId = UserId.NewId();
    private static readonly BookId TestBookId = BookId.NewId();

    [Fact]
    public void Finish_CalculatesPagesRead()
    {
        var session = ReadingSession
            .Start(TestSessionId, TestUserId, TestBookId, 10, DateTime.UtcNow)
            .Value;

        var result = session.Finish(
            25,
            100,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateTime.UtcNow,
            "Notes",
            600
        );

        result.IsSuccess.Should().BeTrue();
        session.PagesRead.Should().Be(15);
    }

    [Fact]
    public void Finish_EndPageCannotBeLowerThanStartPage()
    {
        var session = ReadingSession
            .Start(TestSessionId, TestUserId, TestBookId, 10, DateTime.UtcNow)
            .Value;

        var result = session.Finish(
            5,
            100,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateTime.UtcNow,
            null,
            null
        );

        result.IsFailure.Should().BeTrue();
    }
}
