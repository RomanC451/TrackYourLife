using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.GetGuitarTab;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarTabSearch.Queries.GetGuitarTab;

public sealed class GetGuitarTabQueryHandlerTests
{
    private readonly IUltimateGuitarSearchClient _searchClient = Substitute.For<IUltimateGuitarSearchClient>();
    private readonly GetGuitarTabQueryHandler _handler;

    public GetGuitarTabQueryHandlerTests()
    {
        _handler = new GetGuitarTabQueryHandler(_searchClient);
    }

    [Fact]
    public async Task Handle_LoadsThePublicTabPage()
    {
        var tabUrl = new Uri("https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138");
        var response = new GuitarTabContentResponse(
            5059138,
            "Viu",
            "Bosquito",
            "Chords",
            "C",
            "Standard",
            "E A D G B E",
            null,
            4.5,
            5,
            "[ch]Am[/ch]",
            tabUrl.ToString(),
            [],
            null
        );
        _searchClient.GetTabAsync(tabUrl, Arg.Any<CancellationToken>()).Returns(Result.Success(response));

        var result = await _handler.Handle(
            new GetGuitarTabQuery(tabUrl + "?utm=search"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(response);
    }

    [Fact]
    public async Task Handle_WhenTheUrlIsNotAPublicTab_DoesNotFetchIt()
    {
        var result = await _handler.Handle(
            new GetGuitarTabQuery("https://www.ultimate-guitar.com/pro/?song=viu"),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GuitarTabSearchErrors.InvalidTabUrl);
        await _searchClient.DidNotReceive().GetTabAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>());
    }
}
