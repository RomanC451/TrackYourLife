using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.RecipesDiaries.Commands;

public class AddRecipeDiaryTests
{
    private readonly ISender _sender;
    private readonly AddRecipeDiary _endpoint;

    public AddRecipeDiaryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new AddRecipeDiary(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var recipeId = RecipeId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var entryDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _sender
            .Send(Arg.Any<AddRecipeDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(nutritionDiaryId));

        var request = new AddRecipeDiaryRequest(
            RecipeId: recipeId,
            MealType: MealTypes.Breakfast,
            Quantity: 1.5f,
            EntryDate: entryDate,
            ServingSizeId: servingSizeId
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
                Arg.Is<AddRecipeDiaryCommand>(c =>
                    c.RecipeId == recipeId
                    && c.MealType == MealTypes.Breakfast
                    && Math.Abs(c.Quantity - 1.5f) < 0.000001
                    && c.EntryDate == entryDate
                    && c.ServingSizeId == servingSizeId
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("NotFound", "Recipe not found");
        _sender
            .Send(Arg.Any<AddRecipeDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<NutritionDiaryId>(error));

        var request = new AddRecipeDiaryRequest(
            RecipeId: RecipeId.NewId(),
            MealType: MealTypes.Lunch,
            Quantity: 2.0f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow),
            ServingSizeId: ServingSizeId.NewId()
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
