using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.Reading;
using TrackYourLife.Modules.Reading.Presentation.Features.Books.Commands;
using TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Reading.FunctionalTests.Features;

[Collection("Reading Integration Tests")]
public class ReadingTests(ReadingFunctionalTestWebAppFactory factory)
    : ReadingBaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateBook_WithValidData_ShouldReturnBookId()
    {
        var request = new CreateBookRequest(
            Title: "Test Book",
            Author: "Test Author",
            TotalPages: 200,
            CurrentPage: 0,
            Status: BookStatus.NotStarted,
            StartingDate: null,
            FinishDate: null,
            Rating: null
        );

        var response = await _client.PostAsJsonAsync("/api/books", request);

        var result = await response.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );
        result!.id.Should().NotBe(Guid.Empty);

        var book = await ReadingWriteDbContext.Books.FirstOrDefaultAsync(b =>
            b.Id == BookId.Create(result.id)
        );
        book.Should().NotBeNull();
        book!.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task GetBooks_ShouldReturnUserBooks()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/books",
            new CreateBookRequest(
                "Listed Book",
                "Author",
                100,
                10,
                BookStatus.Ongoing,
                DateOnly.FromDateTime(DateTime.UtcNow),
                null,
                null
            )
        );
        var created = await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        var response = await _client.GetAsync("/api/books");

        var books = await response.ShouldHaveStatusCodeAndContent<List<BookDto>>(
            HttpStatusCode.OK
        );
        books.Should().Contain(b => b.Id == created!.id);
    }

    [Fact]
    public async Task StartReadingSession_WithNotStartedBook_ShouldMarkBookAsOngoing()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/books",
            new CreateBookRequest(
                "New Book",
                "Author",
                100,
                0,
                BookStatus.NotStarted,
                null,
                null,
                null
            )
        );
        var created = await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        var startResponse = await _client.PostAsJsonAsync(
            "/api/reading-sessions",
            new StartReadingSessionRequest(created!.id)
        );
        await startResponse.ShouldHaveStatusCode(HttpStatusCode.Created);

        var book = await ReadingWriteDbContext.Books.FirstAsync(b =>
            b.Id == BookId.Create(created.id)
        );
        book.Status.Should().Be(BookStatus.Ongoing);
        book.StartingDate.Should().NotBeNull();
    }

    [Fact]
    public async Task StartFinishAndCancelSession_FullFlow_ShouldUpdateBookProgress()
    {
        var bookId = await CreateBookAsync(totalPages: 100, currentPage: 10);

        var startResponse = await _client.PostAsJsonAsync(
            "/api/reading-sessions",
            new StartReadingSessionRequest(bookId)
        );
        var started = await startResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        var activeResponse = await _client.GetAsync("/api/reading-sessions/active");
        var active = await activeResponse.ShouldHaveStatusCodeAndContent<ReadingSessionDto>(
            HttpStatusCode.OK
        );
        active!.Id.Should().Be(started!.id);
        active.StartPage.Should().Be(10);

        var cancelResponse = await _client.DeleteAsync(
            $"/api/reading-sessions/{started.id}"
        );
        await cancelResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var restartResponse = await _client.PostAsJsonAsync(
            "/api/reading-sessions",
            new StartReadingSessionRequest(bookId)
        );
        var restarted = await restartResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        var finishResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{restarted!.id}/finish",
            new FinishReadingSessionRequest(
                EndPage: 25,
                SessionDate: DateOnly.FromDateTime(DateTime.UtcNow),
                DurationSeconds: 600
            )
        );
        await finishResponse.ShouldHaveStatusCode(HttpStatusCode.OK);

        var bookResponse = await _client.GetAsync($"/api/books/{bookId}");
        var book = await bookResponse.ShouldHaveStatusCodeAndContent<BookDto>(
            HttpStatusCode.OK
        );
        book!.CurrentPage.Should().Be(25);
    }

    [Fact]
    public async Task AddReadingSessionNotes_ShouldGroupByChapter()
    {
        var bookId = await CreateBookAsync(totalPages: 100, currentPage: 10);
        var sessionId = await StartSessionAsync(bookId);

        var firstNoteResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/notes",
            new { ChapterTitle = "Cap. 4 — Focus", Content = "First note" }
        );
        await firstNoteResponse.ShouldHaveStatusCode(HttpStatusCode.Created);

        var secondNoteResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/notes",
            new { ChapterTitle = "cap. 4 — focus", Content = "Second note" }
        );
        await secondNoteResponse.ShouldHaveStatusCode(HttpStatusCode.Created);

        var thirdNoteResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/notes",
            new { ChapterTitle = "Cap. 3 — Distractions", Content = "Older chapter note" }
        );
        await thirdNoteResponse.ShouldHaveStatusCode(HttpStatusCode.Created);

        var notesResponse = await _client.GetAsync(
            $"/api/reading-sessions/by-book/{bookId}"
        );
        var groups = await notesResponse.ShouldHaveStatusCodeAndContent<
            List<BookChapterNotesGroupDto>
        >(HttpStatusCode.OK);

        groups.Should().HaveCount(2);
        groups![0].ChapterTitle.Should().Be("Cap. 3 — Distractions");
        groups[0].Notes.Should().ContainSingle().Which.Content.Should().Be("Older chapter note");
        groups[1].ChapterTitle.Should().Be("cap. 4 — focus");
        groups[1].Notes.Should().HaveCount(2);
        groups[1].Notes[0].Content.Should().Be("Second note");
        groups[1].Notes[1].Content.Should().Be("First note");
    }

    [Fact]
    public async Task ReadingSessionNotes_CanBeListedUpdatedAndDeleted()
    {
        var bookId = await CreateBookAsync(totalPages: 100, currentPage: 0);
        var sessionId = await StartSessionAsync(bookId);

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/notes",
            new AddReadingSessionNoteRequest("Cap. 1 — Start", "Original note")
        );
        var created = await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        var finishResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/finish",
            new FinishReadingSessionRequest(
                EndPage: 10,
                SessionDate: DateOnly.FromDateTime(DateTime.UtcNow),
                DurationSeconds: 300
            )
        );
        await finishResponse.ShouldHaveStatusCode(HttpStatusCode.OK);

        var listResponse = await _client.GetAsync(
            $"/api/reading-sessions/{sessionId}/notes"
        );
        var notes = await listResponse.ShouldHaveStatusCodeAndContent<
            List<ReadingSessionNoteDto>
        >(HttpStatusCode.OK);

        notes.Should().ContainSingle();
        notes![0].Content.Should().Be("Original note");

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/notes/{created!.id}",
            new UpdateReadingSessionNoteRequest("Cap. 1 — Start", "Updated note")
        );
        await updateResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var updatedListResponse = await _client.GetAsync(
            $"/api/reading-sessions/{sessionId}/notes"
        );
        var updatedNotes = await updatedListResponse.ShouldHaveStatusCodeAndContent<
            List<ReadingSessionNoteDto>
        >(HttpStatusCode.OK);
        updatedNotes![0].Content.Should().Be("Updated note");

        var deleteResponse = await _client.DeleteAsync(
            $"/api/reading-sessions/{sessionId}/notes/{created.id}"
        );
        await deleteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var emptyListResponse = await _client.GetAsync(
            $"/api/reading-sessions/{sessionId}/notes"
        );
        var emptyNotes = await emptyListResponse.ShouldHaveStatusCodeAndContent<
            List<ReadingSessionNoteDto>
        >(HttpStatusCode.OK);
        emptyNotes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReadingPagesHistory_WithFinishedSessions_ShouldReturnAggregatedPages()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var bookId = await CreateBookAsync(totalPages: 100, currentPage: 0);
        var sessionId = await StartSessionAsync(bookId);

        var finishResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/finish",
            new FinishReadingSessionRequest(
                EndPage: 15,
                SessionDate: today,
                DurationSeconds: 300
            )
        );
        await finishResponse.ShouldHaveStatusCode(HttpStatusCode.OK);

        var historyResponse = await _client.GetAsync(
            $"/api/reading/pages-history?overviewType={ReadingOverviewType.Daily}&startDate={today:yyyy-MM-dd}&endDate={today:yyyy-MM-dd}"
        );

        var history = await historyResponse.ShouldHaveStatusCodeAndContent<
            List<ReadingPagesDataPointDto>
        >(HttpStatusCode.OK);

        history.Should().ContainSingle();
        history![0].Pages.Should().Be(15);
    }

    [Fact]
    public async Task DailyProgressAndStreak_WithReadingGoal_ShouldReflectFinishedSessions()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var goalResponse = await _client.PostAsJsonAsync(
            "/api/goals",
            new
            {
                Value = 20,
                Type = GoalType.ReadingPages,
                Period = GoalPeriod.Day,
                StartDate = today,
                Force = false,
                EndDate = (DateOnly?)null,
            }
        );
        await goalResponse.ShouldHaveStatusCode(HttpStatusCode.Created);

        var bookId = await CreateBookAsync(totalPages: 300, currentPage: 0);
        var sessionId = await StartSessionAsync(bookId);

        var finishResponse = await _client.PostAsJsonAsync(
            $"/api/reading-sessions/{sessionId}/finish",
            new FinishReadingSessionRequest(
                EndPage: 20,
                SessionDate: today,
                DurationSeconds: 300
            )
        );
        await finishResponse.ShouldHaveStatusCode(HttpStatusCode.OK);

        var progressResponse = await _client.GetAsync("/api/reading/daily-progress");
        var progress = await progressResponse.ShouldHaveStatusCodeAndContent<DailyReadingProgressDto>(
            HttpStatusCode.OK
        );
        progress!.PagesReadToday.Should().Be(20);
        progress.TargetMet.Should().BeTrue();

        var streakResponse = await _client.GetAsync("/api/reading/streak");
        var streak = await streakResponse.ShouldHaveStatusCodeAndContent<ReadingStreakDto>(
            HttpStatusCode.OK
        );
        streak!.CurrentStreak.Should().BeGreaterThanOrEqualTo(1);
        streak.TodayTargetMet.Should().BeTrue();
    }

    private async Task<Guid> CreateBookAsync(int totalPages, int currentPage)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/books",
            new CreateBookRequest(
                "Flow Book",
                "Author",
                totalPages,
                currentPage,
                BookStatus.Ongoing,
                DateOnly.FromDateTime(DateTime.UtcNow),
                null,
                null
            )
        );
        var result = await response.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );
        return result!.id;
    }

    private async Task<Guid> StartSessionAsync(Guid bookId)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/reading-sessions",
            new StartReadingSessionRequest(bookId)
        );
        var result = await response.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );
        return result!.id;
    }
}
