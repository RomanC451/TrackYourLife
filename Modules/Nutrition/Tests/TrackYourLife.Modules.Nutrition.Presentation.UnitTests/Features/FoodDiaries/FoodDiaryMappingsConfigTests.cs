using Mapster;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.FoodDiaries;

public class FoodDiaryMappingsConfigTests
{
    private readonly TypeAdapterConfig _config;

    public FoodDiaryMappingsConfigTests()
    {
        _config = new TypeAdapterConfig();
        new FoodDiaryMappingsConfig().Register(_config);
    }

    [Fact]
    public void Map_FoodDiaryReadModelToFoodDiaryDto_ShouldMapCorrectly()
    {
        // Arrange
        var food = FoodFaker.Generate();
        var servingSize = ServingSizeFaker.Generate();
        var foodDiary = FoodDiaryFaker.Generate();

        var foodReadModel = new FoodReadModel(
            food.Id,
            food.Name,
            food.Type,
            food.BrandName,
            food.CountryCode
        )
        {
            NutritionalContents = food.NutritionalContents,
        };

        var servingSizeReadModel = new ServingSizeReadModel(
            servingSize.Id,
            servingSize.NutritionMultiplier,
            servingSize.Unit,
            servingSize.Value,
            servingSize.ApiId
        );

        var readModel = new FoodDiaryReadModel(
            foodDiary.Id,
            foodDiary.UserId,
            foodDiary.Quantity,
            foodDiary.MealType,
            foodDiary.Date,
            DateTime.UtcNow
        )
        {
            Food = foodReadModel,
            ServingSize = servingSizeReadModel,
        };

        // Act
        var result = readModel.Adapt<FoodDiaryDto>(_config);

        // Assert
        result.Should().NotBeNull();
        result.Food.Should().NotBeNull();
        result.Food.Name.Should().Be(foodReadModel.Name);
        result.Food.Type.Should().Be(foodReadModel.Type);
        result.Food.BrandName.Should().Be(foodReadModel.BrandName);
        result.Food.CountryCode.Should().Be(foodReadModel.CountryCode);

        result.ServingSize.Should().NotBeNull();
        result.ServingSize.Unit.Should().Be(servingSizeReadModel.Unit);
        result.ServingSize.Value.Should().Be(servingSizeReadModel.Value);
        result
            .ServingSize.NutritionMultiplier.Should()
            .Be(servingSizeReadModel.NutritionMultiplier);
    }

    [Fact]
    public void Map_AddFoodDiaryRequestToAddFoodDiaryCommand_ShouldMapCorrectly()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate();
        var request = new AddFoodDiaryRequest(
            FoodId: foodDiary.FoodId,
            MealType: foodDiary.MealType,
            ServingSizeId: foodDiary.ServingSizeId,
            Quantity: foodDiary.Quantity,
            EntryDate: foodDiary.Date
        );

        // Act
        var result = request.Adapt<AddFoodDiaryCommand>(_config);

        // Assert
        result.Should().NotBeNull();
        result.FoodId.Should().Be(request.FoodId);
        result.ServingSizeId.Should().Be(request.ServingSizeId);
        result.EntryDate.Should().Be(request.EntryDate);
        result.MealType.Should().Be(request.MealType);
        result.Quantity.Should().Be(request.Quantity);
    }

    [Fact]
    public void Map_UpdateFoodDiaryRequestToUpdateFoodDiaryCommand_ShouldMapCorrectly()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate();
        var request = new UpdateFoodDiaryRequest(
            Id: foodDiary.Id,
            Quantity: foodDiary.Quantity,
            ServingSizeId: foodDiary.ServingSizeId,
            MealType: foodDiary.MealType
        );

        // Act
        var result = request.Adapt<UpdateFoodDiaryCommand>(_config);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
        result.Quantity.Should().Be(request.Quantity);
        result.ServingSizeId.Should().Be(request.ServingSizeId);
        result.MealType.Should().Be(request.MealType);
    }
}
