using FluentValidation.TestHelper;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetWatchHistory;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetWatchHistory;

public sealed class GetWatchHistoryQueryValidatorTests
{
    private readonly GetWatchHistoryQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetWatchHistoryQuery(Page: 1, PageSize: 20);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenPageIsZero_ShouldHaveValidationError()
    {
        var query = new GetWatchHistoryQuery(Page: 0, PageSize: 20);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WhenPageSizeIsZero_ShouldHaveValidationError()
    {
        var query = new GetWatchHistoryQuery(Page: 1, PageSize: 0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenPageSizeExceedsMaximum_ShouldHaveValidationError()
    {
        var query = new GetWatchHistoryQuery(Page: 1, PageSize: 51);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
