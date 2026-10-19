using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.SearchGuitarTabs;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarTabSearch.Queries.SearchGuitarTabs;

public sealed class SearchGuitarTabsQueryHandlerTests
{
    private readonly IUltimateGuitarSearchClient _searchClient = Substitute.For<IUltimateGuitarSearchClient>();
    private readonly SearchGuitarTabsQueryHandler _handler;

    public SearchGuitarTabsQueryHandlerTests()
    {
        _handler = new SearchGuitarTabsQueryHandler(_searchClient);
    }

    [Fact]
    public async Task Handle_ReturnsTheSearchPage()
    {
        var response = new GuitarTabSearchResponse("viu", 1, 1, 1, []);
        _searchClient
            .SearchByTitleAsync("viu", 1, Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        var result = await _handler.Handle(new SearchGuitarTabsQuery(" viu ", 1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(response);
    }

    [Fact]
    public async Task Handle_WhenSearchIsUnavailable_ReturnsFailure()
    {
        _searchClient
            .SearchByTitleAsync("viu", 2, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<GuitarTabSearchResponse>(GuitarTabSearchErrors.Unavailable));

        var result = await _handler.Handle(new SearchGuitarTabsQuery("viu", 2), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GuitarTabSearchErrors.Unavailable);
    }
}
