using FluentValidation.TestHelper;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Queries.GetChannelsByCategory;

public sealed class GetChannelsByCategoryQueryValidatorTests
{
    private readonly GetChannelsByCategoryQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetChannelsByCategoryQuery(YoutubeCategoryId.NewId());

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenFavoritesOnly_ShouldNotHaveValidationErrors()
    {
        var query = new GetChannelsByCategoryQuery(null, FavoritesOnly: true);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenFavoritesOnlyAndCategorySpecified_ShouldHaveValidationError()
    {
        var query = new GetChannelsByCategoryQuery(YoutubeCategoryId.NewId(), FavoritesOnly: true);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_WhenCategoryIdIsEmpty_ShouldHaveValidationError()
    {
        var query = new GetChannelsByCategoryQuery(YoutubeCategoryId.Empty);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.YoutubeCategoryId);
    }
}
