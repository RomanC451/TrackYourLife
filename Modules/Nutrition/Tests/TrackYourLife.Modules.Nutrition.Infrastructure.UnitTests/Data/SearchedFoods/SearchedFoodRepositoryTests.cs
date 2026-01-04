using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.SearchedFoods;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.SearchedFoods;

public class SearchedFoodRepositoryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private SearchedFoodRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new SearchedFoodRepository(WriteDbContext);
    }

    [Fact]
    public async Task GetByNameAsync_WhenSearchedFoodExists_ShouldReturnSearchedFood()
    {
        // Arrange
        var searchedFood = SearchedFood.Create(SearchedFoodId.NewId(), "test food").Value;

        await WriteDbContext.SearchedFoods.AddAsync(searchedFood);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByNameAsync("test food", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("test food");
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByNameAsync_WhenSearchedFoodDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var searchedFood = SearchedFood.Create(SearchedFoodId.NewId(), "test food").Value;

        await WriteDbContext.SearchedFoods.AddAsync(searchedFood);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByNameAsync("non-existent food", CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByNameAsync_ShouldNotBeCaseSensitive()
    {
        // Arrange
        var searchedFood = SearchedFood.Create(SearchedFoodId.NewId(), "test food").Value;

        await WriteDbContext.SearchedFoods.AddAsync(searchedFood);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByNameAsync("TEST FOOD", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("test food");
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
