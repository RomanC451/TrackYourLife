using NSubstitute;
using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Events;

public class RecipeDiaryDeletedDomainEventHandlerTests
{
    private readonly UserId _userId;
    private readonly DateOnly _date;
    private readonly RecipeId _recipeId;

    public RecipeDiaryDeletedDomainEventHandlerTests()
    {
        _userId = UserId.NewId();
        _date = DateOnly.FromDateTime(DateTime.Today);
        _recipeId = RecipeId.NewId();
    }

    [Fact]
    public async Task Handle_WhenRecipeNotFound_ShouldThrowEventFailedException()
    {
        // Arrange
        var queryRepository = Substitute.For<IRecipeRepository>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        queryRepository.GetByIdAsync(_recipeId, default).Returns((Recipe?)null);

        var handler = new RecipeDiaryDeletedDomainEventHandler(
            dailyNutritionOverviewRepository,
            queryRepository,
            unitOfWork,
            logger
        );

        var domainEvent = new RecipeDiaryDeletedDomainEvent(
            _userId,
            _date,
            _recipeId,
            1,
            ServingSizeId.NewId()
        );

        // Act & Assert
        await Assert.ThrowsAsync<EventFailedException>(() => handler.Handle(domainEvent, default));
        
        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_WhenOverviewNotFound_ShouldThrowEventFailedException()
    {
        // Arrange
        var queryRepository = Substitute.For<IRecipeRepository>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var recipe = Recipe.Create(_recipeId, _userId, "Test Recipe", 100f, 1).Value;
        var nutritionalContent = new NutritionalContent
        {
            Energy = new Energy { Value = 400, Unit = "Kcal" },
            Carbohydrates = 80,
            Fat = 20,
            Protein = 8,
        };
        recipe.NutritionalContents.AddNutritionalValues(nutritionalContent);

        queryRepository.GetByIdAsync(_recipeId, default).Returns(recipe);
        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns((DailyNutritionOverview?)null);

        var logger = Substitute.For<ILogger>();
        var handler = new RecipeDiaryDeletedDomainEventHandler(
            dailyNutritionOverviewRepository,
            queryRepository,
            unitOfWork,
            logger
        );

        var domainEvent = new RecipeDiaryDeletedDomainEvent(
            _userId,
            _date,
            _recipeId,
            1,
            ServingSizeId.NewId()
        );

        // Act & Assert
        await Assert.ThrowsAsync<EventFailedException>(() => handler.Handle(domainEvent, default));

        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_WhenAllDataFound_ShouldUpdateOverview()
    {
        // Arrange
        DailyNutritionOverview updatedOverview = null!;

        var queryRepository = Substitute.For<IRecipeRepository>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var recipe = Recipe.Create(_recipeId, _userId, "Test Recipe", 100f, 1).Value;
        var nutritionalContent = new NutritionalContent
        {
            Energy = new Energy { Value = 400, Unit = "Kcal" },
            Carbohydrates = 80,
            Fat = 20,
            Protein = 8,
        };
        recipe.NutritionalContents.AddNutritionalValues(nutritionalContent);

        var overview = DailyNutritionOverview
            .Create(DailyNutritionOverviewId.NewId(), _userId, _date, 2000, 200, 50, 100)
            .Value;

        overview.NutritionalContent.AddNutritionalValues(
            new NutritionalContent
            {
                Energy = new Energy { Value = 2000, Unit = "Kcal" },
                Carbohydrates = 200,
                Fat = 50,
                Protein = 100,
            }
        );

        queryRepository.GetByIdAsync(_recipeId, default).Returns(recipe);
        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns(overview);

        dailyNutritionOverviewRepository
            .WhenForAnyArgs(x => x.Update(Arg.Any<DailyNutritionOverview>()))
            .Do(x => updatedOverview = x.Arg<DailyNutritionOverview>());

        var logger = Substitute.For<ILogger>();
        var handler = new RecipeDiaryDeletedDomainEventHandler(
            dailyNutritionOverviewRepository,
            queryRepository,
            unitOfWork,
            logger
        );

        // Get the first serving size from the recipe (portions serving size)
        var servingSizeId = recipe.ServingSizes[0].Id;

        var domainEvent = new RecipeDiaryDeletedDomainEvent(
            _userId,
            _date,
            _recipeId,
            2, // Quantity of 2 portions
            servingSizeId
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(updatedOverview);
        await unitOfWork.Received(1).SaveChangesAsync(default);

        var portionMultiplier = 2f / recipe.Portions;
        var expectedCalories = 2000 - (400 * portionMultiplier);
        var expectedCarbohydrates = 200 - (80 * portionMultiplier);
        var expectedFats = 50 - (20 * portionMultiplier);
        var expectedProtein = 100 - (8 * portionMultiplier);

        updatedOverview.NutritionalContent.Energy.Value.Should().Be(expectedCalories);
        updatedOverview.NutritionalContent.Carbohydrates.Should().Be(expectedCarbohydrates);
        updatedOverview.NutritionalContent.Fat.Should().Be(expectedFats);
        updatedOverview.NutritionalContent.Protein.Should().Be(expectedProtein);
    }
}
