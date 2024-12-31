using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.SearchedFoods;

public class SearchedFoodTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateSearchedFood()
    {
        // Arrange
        var id = SearchedFoodId.NewId();
        var name = "Lapte";

        // Act
        var searchedFood = SearchedFood.Create(id, name).Value;

        // Assert
        searchedFood.Should().NotBeNull();
        searchedFood.Id.Should().Be(id);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var id = SearchedFoodId.Empty;
        var name = "Lapte";

        // Act
        var result = SearchedFood.Create(id, name);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Id");
        result.Error.Code.Should().Contain(nameof(SearchedFood));
    }

    [Fact]
    public void Create_WithEmptyName_ShouldReturnFailure()
    {
        // Arrange
        var id = SearchedFoodId.NewId();
        var name = string.Empty;

        // Act
        var result = SearchedFood.Create(id, name);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Name");
        result.Error.Code.Should().Contain(nameof(SearchedFood));
    }
}
