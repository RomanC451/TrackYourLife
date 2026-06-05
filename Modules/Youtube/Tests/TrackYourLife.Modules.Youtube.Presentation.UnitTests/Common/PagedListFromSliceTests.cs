using TrackYourLife.SharedLib.Contracts.Common;
using Xunit;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Common;

public sealed class PagedListFromSliceTests
{
    [Fact]
    public void FromSlice_WithItems_ShouldReturnPagedListMetadata()
    {
        var result = PagedList<string>.FromSlice(["a", "b"], page: 1, pageSize: 2, totalCount: 5);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().Equal("a", "b");
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(2);
        result.Value.MaxPage.Should().Be(3);
        result.Value.HasNextPage.Should().BeTrue();
        result.Value.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void FromSlice_WithEmptyTotalCount_ShouldAllowFirstPageOnly()
    {
        var result = PagedList<string>.FromSlice([], page: 1, pageSize: 20, totalCount: 0);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.MaxPage.Should().Be(1);
        result.Value.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void FromSlice_WhenPageIsOutOfRange_ShouldReturnFailure()
    {
        var result = PagedList<string>.FromSlice([], page: 2, pageSize: 20, totalCount: 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("OutOfIndex");
    }
}
