using FluentAssertions;
using Mapster;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryByDate;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetTotalCaloriesByPeriod;
using TrackYourLife.Modules.Nutrition.Application.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries;

public class FoodDiaryMappingsConfigTests
{
    private readonly INutritionMapper mapper = NutritionMapperHelper.CreateMapper();

    [Fact]
    public void Register_ShouldMapFoodDiaryReadModelToFoodDiaryDto()
    {
        // Arrange
        var foodDiaryReadModel = FoodDiaryFaker.GenerateReadModel();

        var foodDto = mapper.Map<FoodDto>(foodDiaryReadModel.Food);

        // Act
        var result = mapper.Map<FoodDiaryDto>(foodDiaryReadModel);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(foodDiaryReadModel.Id);
        result.Food.Should().BeEquivalentTo(foodDto);
        result.MealType.Should().Be(foodDiaryReadModel.MealType);
        result.Quantity.Should().Be(foodDiaryReadModel.Quantity);
        result.ServingSize.Should().BeEquivalentTo(foodDiaryReadModel.ServingSize);
        result.Date.Should().Be(foodDiaryReadModel.Date);
    }

    // TODO: move this

    //[Fact]
    //public void Register_ShouldMapAddFoodDiaryRequestToAddFoodDiaryCommand()
    //{
    //    // Arrange
    //    var addFoodDiaryRequest = new AddFoodDiaryRequest(
    //        FoodId.NewId(),
    //        MealTypes.Breakfast,
    //        ServingSizeId.NewId(),
    //        100,
    //        DateOnly.FromDateTime(DateTime.UtcNow)
    //    );

    //    // Act
    //    var result = mapper.Map<AddFoodDiaryCommand>(addFoodDiaryRequest);

    //    // Assert
    //    result.Should().NotBeNull();
    //    result.FoodId.Should().Be(addFoodDiaryRequest.FoodId);
    //    result.MealType.Should().Be(addFoodDiaryRequest.MealType);
    //    result.ServingSizeId.Should().Be(addFoodDiaryRequest.ServingSizeId);
    //    result.Quantity.Should().Be(addFoodDiaryRequest.Quantity);
    //    result.EntryDate.Should().Be(addFoodDiaryRequest.EntryDate);
    //}

    //[Fact]
    //public void Register_ShouldMapUpdateFoodDiaryRequestToUpdateFoodDiaryCommand()
    //{
    //    // Arrange
    //    var updateFoodDiaryRequest = new UpdateFoodDiaryRequest(
    //        NutritionDiaryId.NewId(),
    //        10,
    //        ServingSizeId.NewId(),
    //        MealTypes.Breakfast
    //    );

    //    // Act
    //    var result = mapper.Map<UpdateFoodDiaryCommand>(updateFoodDiaryRequest);

    //    // Assert
    //    result.Should().NotBeNull();
    //    result.Id.Should().Be(updateFoodDiaryRequest.Id);
    //    result.Quantity.Should().Be(updateFoodDiaryRequest.Quantity);
    //    result.ServingSizeId.Should().Be(updateFoodDiaryRequest.ServingSizeId);
    //}

    //[Fact]
    //public void Register_ShouldMapGetFoodDiaryByDateRequestToGetFoodDiaryByDateQuery()
    //{
    //    // Arrange
    //    var getFoodDiaryByDateRequest = new GetFoodDiaryByDateRequest(
    //        DateOnly.FromDateTime(DateTime.UtcNow)
    //    );

    //    // Act
    //    var result = mapper.Map<GetFoodDiaryByDateQuery>(getFoodDiaryByDateRequest);

    //    // Assert
    //    result.Should().NotBeNull();
    //    result.Day.Should().Be(getFoodDiaryByDateRequest.Day);
    //}

    //[Fact]
    //public void Register_ShouldMapGetTotalCaloriesByPeriodRequestToGetTotalCaloriesByPeriodQuery()
    //{
    //    // Arrange
    //    var getTotalCaloriesByPeriodRequest = new GetTotalCaloriesByPeriodRequest(
    //        DateOnly.FromDateTime(DateTime.UtcNow),
    //        DateOnly.FromDateTime(DateTime.UtcNow)
    //    );

    //    // Act
    //    var result = mapper.Map<GetTotalCaloriesByPeriodQuery>(getTotalCaloriesByPeriodRequest);

    //    // Assert
    //    result.Should().NotBeNull();
    //    result.StartDate.Should().Be(getTotalCaloriesByPeriodRequest.StartDate);
    //    result.EndDate.Should().Be(getTotalCaloriesByPeriodRequest.EndDate);
    //}
}
