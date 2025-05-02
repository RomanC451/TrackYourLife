using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.Foods;

public class FoodTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateFood()
    {
        // Arrange
        var id = FoodId.NewId();
        var type = "Food";
        var brandName = "Test Brand";
        var countryCode = "US";
        var name = "Test Food";
        var nutritionalContents = new NutritionalContent();
        var foodServingSize = FoodServingSize
            .Create(FoodId.NewId(), ServingSizeId.NewId(), 0)
            .Value;

        // Act
        var food = Food.Create(
            id,
            type,
            brandName,
            countryCode,
            name,
            nutritionalContents,
            [foodServingSize]
        ).Value;

        // Assert
        food.Should().NotBeNull();
        food.Id.Should().Be(id);
        food.Type.Should().Be(type);
        food.BrandName.Should().Be(brandName);
        food.CountryCode.Should().Be(countryCode);
        food.Name.Should().Be(name);
        food.NutritionalContents.Should().Be(nutritionalContents);
        food.FoodServingSizes.Should().Contain(foodServingSize);
    }

    [Fact]
    public void Create_WithInvalidId_ShouldFail()
    {
        // Arrange
        var type = "Food";
        var brandName = "Test Brand";
        var countryCode = "US";
        var name = "Test Food";
        var nutritionalContents = new NutritionalContent();
        var foodServingSize = FoodServingSize
            .Create(FoodId.NewId(), ServingSizeId.NewId(), 0)
            .Value;

        // Act
        var result = Food.Create(
            FoodId.Empty,
            type,
            brandName,
            countryCode,
            name,
            nutritionalContents,
            [foodServingSize]
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Id");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void Create_WithInvalidType_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();
        var brandName = "Test Brand";
        var countryCode = "US";
        var name = "Test Food";
        var nutritionalContents = new NutritionalContent();
        var foodServingSize = FoodServingSize
            .Create(FoodId.NewId(), ServingSizeId.NewId(), 0)
            .Value;

        // Act
        var result = Food.Create(
            id,
            string.Empty,
            brandName,
            countryCode,
            name,
            nutritionalContents,
            [foodServingSize]
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Type");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void Create_WithInvalidName_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();
        var type = "Food";
        var brandName = "Test Brand";
        var countryCode = "US";
        var nutritionalContents = new NutritionalContent();
        var foodServingSize = FoodServingSize
            .Create(FoodId.NewId(), ServingSizeId.NewId(), 0)
            .Value;

        // Act
        var result = Food.Create(
            id,
            type,
            brandName,
            countryCode,
            string.Empty,
            nutritionalContents,
            [foodServingSize]
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Name");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void Create_WithInvalidNutritionalContents_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();
        var type = "Food";
        var brandName = "Test Brand";
        var countryCode = "US";
        var name = "Test Food";
        var foodServingSize = FoodServingSize
            .Create(FoodId.NewId(), ServingSizeId.NewId(), 0)
            .Value;

        // Act
        var result = Food.Create(id, type, brandName, countryCode, name, null!, [foodServingSize]);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Null");
        result.Error.Code.Should().Contain("NutritionalContents");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void Create_WithInvalidFoodServingSizes_ShouldFail()
    {
        // Arrange
        var id = FoodId.NewId();
        var type = "Food";
        var brandName = "Test Brand";
        var countryCode = "US";
        var name = "Test Food";
        var nutritionalContents = new NutritionalContent();

        // Act
        var result = Food.Create(id, type, brandName, countryCode, name, nutritionalContents, []);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("FoodServingSizes");
        result.Error.Code.Should().Contain(nameof(Food));
    }
}
