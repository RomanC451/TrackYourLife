using FluentValidation.TestHelper;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetAllLatestVideos;

public sealed class GetAllLatestVideosQueryValidatorTests
{
    private readonly GetAllLatestVideosQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetAllLatestVideosQuery(MaxResultsPerChannel: 5, YoutubeCategoryId: YoutubeCategoryId.NewId());

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenFavoritesOnly_ShouldNotHaveValidationErrors()
    {
        var query = new GetAllLatestVideosQuery(FavoritesOnly: true);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenFavoritesOnlyAndCategorySpecified_ShouldHaveValidationError()
    {
        var query = new GetAllLatestVideosQuery(
            YoutubeCategoryId: YoutubeCategoryId.NewId(),
            FavoritesOnly: true
        );

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_WhenCategoryIdIsEmpty_ShouldHaveValidationError()
    {
        var query = new GetAllLatestVideosQuery(YoutubeCategoryId: YoutubeCategoryId.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.YoutubeCategoryId);
    }

    [Fact]
    public void Validate_WhenMaxResultsPerChannelOutOfRange_ShouldHaveValidationError()
    {
        var query = new GetAllLatestVideosQuery(MaxResultsPerChannel: 0);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.MaxResultsPerChannel);
    }
}
