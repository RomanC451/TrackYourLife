using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.FoodDiaries.Commands;

public class AddFoodDiaryTests
{
    private readonly ISender _sender;
    private readonly AddFoodDiary _endpoint;

    public AddFoodDiaryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new AddFoodDiary(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var entryDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _sender
            .Send(Arg.Any<AddFoodDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(nutritionDiaryId));

        var request = new AddFoodDiaryRequest(
            FoodId: foodId,
            MealType: MealTypes.Breakfast,
            ServingSizeId: servingSizeId,
            Quantity: 1.5f,
            EntryDate: entryDate
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(nutritionDiaryId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<AddFoodDiaryCommand>(c =>
                    c.FoodId == foodId
                    && c.MealType == MealTypes.Breakfast
                    && c.ServingSizeId == servingSizeId
                    && Math.Abs(c.Quantity - 1.5f) < 0.000001
                    && c.EntryDate == entryDate
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("NotFound", "Food not found");
        _sender
            .Send(Arg.Any<AddFoodDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<NutritionDiaryId>(error));

        var request = new AddFoodDiaryRequest(
            FoodId: FoodId.NewId(),
            MealType: MealTypes.Lunch,
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 2.0f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapRequestToCommandCorrectly()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var entryDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _sender
            .Send(Arg.Any<AddFoodDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(nutritionDiaryId));

        var request = new AddFoodDiaryRequest(
            FoodId: foodId,
            MealType: MealTypes.Dinner,
            ServingSizeId: servingSizeId,
            Quantity: 3.0f,
            EntryDate: entryDate
        );

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<AddFoodDiaryCommand>(c =>
                    c.FoodId == foodId
                    && c.MealType == MealTypes.Dinner
                    && c.ServingSizeId == servingSizeId
                    && Math.Abs(c.Quantity - 3.0f) < 0.000001
                    && c.EntryDate == entryDate
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
