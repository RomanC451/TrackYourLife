using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.FoodDiaries.Commands;

public class UpdateFoodDiaryTests
{
    private readonly ISender _sender;
    private readonly UpdateFoodDiary _endpoint;

    public UpdateFoodDiaryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateFoodDiary(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var entryDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", nutritionDiaryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<UpdateFoodDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new UpdateFoodDiaryRequest(
            MealType: MealTypes.Breakfast,
            ServingSizeId: servingSizeId,
            Quantity: 2.0f,
            EntryDate: entryDate
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateFoodDiaryCommand>(c =>
                    c.MealType == MealTypes.Breakfast
                    && c.ServingSizeId == servingSizeId
                    && Math.Abs(c.Quantity - 2.0f) < 0.000001
                    && c.EntryDate == entryDate
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", nutritionDiaryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Food diary not found");
        _sender
            .Send(Arg.Any<UpdateFoodDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new UpdateFoodDiaryRequest(
            MealType: MealTypes.Lunch,
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 1.5f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
