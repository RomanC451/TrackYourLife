using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.SearchGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.UnitTests.Features.GuitarTabSearch.Queries.SearchGuitarTabs;

public sealed class SearchGuitarTabsQueryValidatorTests
{
    private readonly SearchGuitarTabsQueryValidator _validator = new();

    [Fact]
    public void Validate_WithTitleAndPage_IsValid()
    {
        var result = _validator.Validate(new SearchGuitarTabsQuery("viu", 1));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithoutSearchText_IsInvalid(string value)
    {
        var result = _validator.Validate(new SearchGuitarTabsQuery(value, 1));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithPageBelowOne_IsInvalid()
    {
        var result = _validator.Validate(new SearchGuitarTabsQuery("viu", 0));

        result.IsValid.Should().BeFalse();
    }
}
