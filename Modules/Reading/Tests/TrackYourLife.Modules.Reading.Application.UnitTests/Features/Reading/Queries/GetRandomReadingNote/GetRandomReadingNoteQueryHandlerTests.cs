using TrackYourLife.Modules.Reading.Application.Abstraction;
using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetRandomReadingNote;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Application.UnitTests.Features.Reading.Queries.GetRandomReadingNote;

public class GetRandomReadingNoteQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IReadingSessionNotesQuery _readingSessionNotesQuery;
    private readonly GetRandomReadingNoteQueryHandler _handler;
    private readonly UserId _userId;

    public GetRandomReadingNoteQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _readingSessionNotesQuery = Substitute.For<IReadingSessionNotesQuery>();
        _handler = new GetRandomReadingNoteQueryHandler(
            _readingSessionNotesQuery,
            _userIdentifierProvider
        );
        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoteExists_ShouldReturnMappedDto()
    {
        var noteId = ReadingSessionNoteId.NewId();
        var bookId = BookId.NewId();
        var note = new RandomReadingNoteReadModel(
            noteId,
            bookId,
            "Flow Book",
            "Cap. 1 — Start",
            "A memorable quote"
        );

        _readingSessionNotesQuery
            .GetRandomByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(note);

        var result = await _handler.Handle(new GetRandomReadingNoteQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.NoteId.Should().Be(noteId.Value);
        result.Value.BookId.Should().Be(bookId.Value);
        result.Value.BookTitle.Should().Be("Flow Book");
        result.Value.ChapterTitle.Should().Be("Cap. 1 — Start");
        result.Value.Content.Should().Be("A memorable quote");
    }

    [Fact]
    public async Task Handle_WhenNoNotesExist_ShouldReturnNoneAvailableFailure()
    {
        _readingSessionNotesQuery
            .GetRandomByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((RandomReadingNoteReadModel?)null);

        var result = await _handler.Handle(new GetRandomReadingNoteQuery(), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReadingSessionNoteErrors.NoneAvailable);
    }
}
