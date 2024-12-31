using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;
using TrackYourLife.Modules.Nutrition.Application.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Foods;

public class FoodMappingsConfigTests
{
    private readonly INutritionMapper mapper = NutritionMapperHelper.CreateMapper();

    [Fact]
    public void ShouldMapServingSizeToServingSizeDto()
    {
        //Arrange
        var servingSize = ServingSizeFaker.Generate();

        //Act
        var result = mapper.Map<ServingSizeDto>(servingSize);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(servingSize.Id);
        result.NutritionMultiplier.Should().Be(servingSize.NutritionMultiplier);
        result.Unit.Should().Be(servingSize.Unit);
        result.Value.Should().Be(servingSize.Value);
    }

    [Fact]
    public void ShouldMapFoodReadModelToFoodDto()
    {
        //Arrange
        var foodReadModel = FoodFaker.GenerateReadModel();

        //Act
        var result = mapper.Map<FoodDto>(foodReadModel);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(foodReadModel.Id);
        result.Type.Should().Be(foodReadModel.Type);
        result.BrandName.Should().Be(foodReadModel.BrandName);
        result.CountryCode.Should().Be(foodReadModel.CountryCode);
        result.Name.Should().Be(foodReadModel.Name);
        result.NutritionalContents.Should().BeEquivalentTo(foodReadModel.NutritionalContents);

        result.ServingSizes.Should().NotBeNull();
        result.ServingSizes.Should().HaveCount(foodReadModel.FoodServingSizes.Count);
        foreach (var foodServingSize in foodReadModel.FoodServingSizes)
        {
            result.ServingSizes[foodServingSize.Index].Should().NotBeNull();
            result
                .ServingSizes[foodServingSize.Index]
                .Id.Should()
                .Be(foodServingSize.ServingSizeId);
            result
                .ServingSizes[foodServingSize.Index]
                .NutritionMultiplier.Should()
                .Be(foodServingSize.ServingSize.NutritionMultiplier);
            result
                .ServingSizes[foodServingSize.Index]
                .Unit.Should()
                .Be(foodServingSize.ServingSize.Unit);
            result
                .ServingSizes[foodServingSize.Index]
                .Value.Should()
                .Be(foodServingSize.ServingSize.Value);
        }
    }

    [Fact]
    public void ShouldMapFoodToFoodDto()
    {
        //Arrange
        var food = FoodFaker.Generate();

        //Act
        var result = mapper.Map<FoodDto>(food);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(food.Id);
        result.Type.Should().Be(food.Type);
        result.BrandName.Should().Be(food.BrandName);
        result.CountryCode.Should().Be(food.CountryCode);
        result.Name.Should().Be(food.Name);
        result.NutritionalContents.Should().BeEquivalentTo(food.NutritionalContents);

        result.ServingSizes.Should().NotBeNull();
        result.ServingSizes.Should().HaveCount(food.FoodServingSizes.Count);
        foreach (var foodServingSize in food.FoodServingSizes)
        {
            result.ServingSizes[foodServingSize.Index].Should().NotBeNull();
            result
                .ServingSizes[foodServingSize.Index]
                .Id.Should()
                .Be(foodServingSize.ServingSizeId);
            result
                .ServingSizes[foodServingSize.Index]
                .NutritionMultiplier.Should()
                .Be(foodServingSize.ServingSize.NutritionMultiplier);
            result
                .ServingSizes[foodServingSize.Index]
                .Unit.Should()
                .Be(foodServingSize.ServingSize.Unit);
            result
                .ServingSizes[foodServingSize.Index]
                .Value.Should()
                .Be(foodServingSize.ServingSize.Value);
        }
    }
}
