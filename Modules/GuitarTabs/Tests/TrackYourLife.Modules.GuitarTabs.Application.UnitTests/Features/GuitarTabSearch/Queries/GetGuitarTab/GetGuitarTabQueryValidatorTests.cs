using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.GetGuitarTab;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarTabSearch.Queries.GetGuitarTab;

public sealed class GetGuitarTabQueryValidatorTests
{
    private readonly GetGuitarTabQueryValidator _validator = new();

    [Fact]
    public void Validate_WithAPublicTabUrl_IsValid()
    {
        var result = _validator.Validate(
            new GetGuitarTabQuery("https://tabs.ultimate-guitar.com/tab/bosquito/viu-chords-5059138")
        );

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("https://example.com/tab/song")]
    [InlineData("https://www.ultimate-guitar.com/pro/?song=viu")]
    public void Validate_WithANonTabUrl_IsInvalid(string url)
    {
        var result = _validator.Validate(new GetGuitarTabQuery(url));

        result.IsValid.Should().BeFalse();
    }
}
