using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.UnitTests.Features.Books;

public class BookTests
{
    private static readonly BookId TestBookId = BookId.NewId();
    private static readonly UserId TestUserId = UserId.NewId();

    [Fact]
    public void Create_WithFinishedStatus_RequiresFinishDateAndRating()
    {
        var result = Book.Create(
            TestBookId,
            TestUserId,
            "Title",
            "Author",
            100,
            100,
            BookStatus.Finished,
            DateOnly.FromDateTime(DateTime.UtcNow),
            finishDate: null,
            rating: null
        );

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithValidFinishedBook_Succeeds()
    {
        var result = Book.Create(
            TestBookId,
            TestUserId,
            "Title",
            "Author",
            100,
            100,
            BookStatus.Finished,
            DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow),
            5
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Rating.Should().Be(5);
    }

    [Fact]
    public void UpdateCurrentPage_CannotExceedTotalPages()
    {
        var book = Book
            .Create(
                TestBookId,
                TestUserId,
                "Title",
                "Author",
                100,
                0,
                BookStatus.Ongoing,
                DateOnly.FromDateTime(DateTime.UtcNow),
                null,
                null
            )
            .Value;

        book.UpdateCurrentPage(101).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void BeginReading_FromNotStarted_SetsOngoingAndStartingDate()
    {
        var startingDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var book = Book
            .Create(
                TestBookId,
                TestUserId,
                "Title",
                "Author",
                100,
                0,
                BookStatus.NotStarted,
                null,
                null,
                null
            )
            .Value;

        book.BeginReading(startingDate).IsSuccess.Should().BeTrue();
        book.Status.Should().Be(BookStatus.Ongoing);
        book.StartingDate.Should().Be(startingDate);
    }

    [Fact]
    public void BeginReading_WhenAlreadyOngoing_IsIdempotent()
    {
        var startingDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var book = Book
            .Create(
                TestBookId,
                TestUserId,
                "Title",
                "Author",
                100,
                10,
                BookStatus.Ongoing,
                startingDate,
                null,
                null
            )
            .Value;

        book.BeginReading(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
            .IsSuccess.Should()
            .BeTrue();
        book.Status.Should().Be(BookStatus.Ongoing);
        book.StartingDate.Should().Be(startingDate);
    }

    [Fact]
    public void SuggestMarkAsFinished_WhenAtLastPage_ReturnsTrue()
    {
        var book = Book
            .Create(
                TestBookId,
                TestUserId,
                "Title",
                "Author",
                100,
                100,
                BookStatus.Ongoing,
                DateOnly.FromDateTime(DateTime.UtcNow),
                null,
                null
            )
            .Value;

        book.SuggestMarkAsFinished().Should().BeTrue();
    }
}
