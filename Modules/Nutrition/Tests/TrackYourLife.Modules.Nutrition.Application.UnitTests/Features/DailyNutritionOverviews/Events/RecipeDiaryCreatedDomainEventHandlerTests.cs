using MassTransit;
using MassTransit.Testing;
using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Events;

public class RecipeDiaryCreatedDomainEventHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private readonly UserId _userId;
    private readonly DateOnly _date;
    private readonly RecipeId _recipeId;
    private readonly float _quantity;

    public RecipeDiaryCreatedDomainEventHandlerTests()
    {
        _userId = UserId.NewId();
        _date = DateOnly.FromDateTime(DateTime.Today);
        _recipeId = RecipeId.NewId();
        _quantity = 1.0f;
    }

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();

        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "fake-users",
                e =>
                {
                    e.Handler<GetNutritionGoalsByUserIdRequest>(async context =>
                    {
                        await context.RespondAsync(
                            new GetNutritionGoalsByUserIdResponse(
                                new NutritionGoals(2000, 250, 70, 150),
                                []
                            )
                        );
                    });
                }
            );
        };

        await _harness.Start();
        _bus = _harness.Bus;
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
    }

    [Fact]
    public async Task Handle_WhenRecipeNotFound_ShouldNotUpdateOverview()
    {
        // Arrange
        var recipeQuery = Substitute.For<IRecipeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        recipeQuery.GetByIdAsync(_recipeId, default).Returns((RecipeReadModel?)null);

        var handler = new RecipeDiaryCreatedDomainEventHandler(
            dailyNutritionOverviewRepository,
            recipeQuery,
            unitOfWork,
            _bus,
            logger
        );

        var domainEvent = new RecipeDiaryCreatedDomainEvent(
            _userId,
            _recipeId,
            _date,
            _quantity,
            ServingSizeId.NewId()
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        await dailyNutritionOverviewRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<DailyNutritionOverview>(), default);
        await unitOfWork.DidNotReceive().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_WhenOverviewExists_ShouldUpdateOverview()
    {
        // Arrange
        DailyNutritionOverview updatedOverview = null!;

        var recipeQuery = Substitute.For<IRecipeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        var existingOverview = DailyNutritionOverviewFaker.Generate(
            userId: _userId,
            date: _date,
            caloriesGoal: 2000,
            carbohydratesGoal: 200,
            fatGoal: 50,
            proteinGoal: 100
        );

        var recipe = new RecipeReadModel(
            Id: _recipeId,
            UserId: _userId,
            Name: "Test Recipe",
            Portions: 2,
            Weight: 100f,
            IsOld: false
        )
        {
            NutritionalContents = new NutritionalContent
            {
                Energy = new Energy { Value = 100 },
                Protein = 10,
                Fat = 5,
                Carbohydrates = 2,
            },
            ServingSizes = [new ServingSizeReadModel(ServingSizeId.NewId(), 1, "g", 10, null)],
        };

        recipeQuery.GetByIdAsync(_recipeId, default).Returns(recipe);
        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns(existingOverview);

        dailyNutritionOverviewRepository
            .WhenForAnyArgs(x => x.Update(Arg.Any<DailyNutritionOverview>()))
            .Do(x => updatedOverview = x.Arg<DailyNutritionOverview>());

        var handler = new RecipeDiaryCreatedDomainEventHandler(
            dailyNutritionOverviewRepository,
            recipeQuery,
            unitOfWork,
            _bus,
            logger
        );

        var domainEvent = new RecipeDiaryCreatedDomainEvent(
            _userId,
            _recipeId,
            _date,
            _quantity,
            recipe.ServingSizes[0].Id
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(updatedOverview);
        await unitOfWork.Received(1).SaveChangesAsync(default);

        updatedOverview.Should().NotBeNull();
        updatedOverview.Id.Should().Be(existingOverview.Id);
        updatedOverview.UserId.Should().Be(existingOverview.UserId);
        updatedOverview.Date.Should().Be(existingOverview.Date);
    }

    [Fact]
    public async Task Handle_WhenOverviewDoesNotExist_ShouldCreateNewOverview()
    {
        // Arrange
        var servingSizeId = ServingSizeId.NewId();
        var recipeQuery = Substitute.For<IRecipeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        var recipe = new RecipeReadModel(
            Id: _recipeId,
            UserId: _userId,
            Name: "Test Recipe",
            Portions: 2,
            Weight: 100f,
            IsOld: false
        )
        {
            NutritionalContents = new NutritionalContent
            {
                Energy = new Energy { Value = 100 },
                Protein = 10,
                Fat = 5,
                Carbohydrates = 2,
            },
            ServingSizes = [new ServingSizeReadModel(servingSizeId, 1, "g", 10, null)],
        };

        recipeQuery.GetByIdAsync(_recipeId, default).Returns(recipe);
        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns((DailyNutritionOverview?)null);

        var handler = new RecipeDiaryCreatedDomainEventHandler(
            dailyNutritionOverviewRepository,
            recipeQuery,
            unitOfWork,
            _bus,
            logger
        );

        var domainEvent = new RecipeDiaryCreatedDomainEvent(
            _userId,
            _recipeId,
            _date,
            _quantity,
            servingSizeId
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        await dailyNutritionOverviewRepository
            .Received(1)
            .AddAsync(
                Arg.Is<DailyNutritionOverview>(overview =>
                    overview.UserId == _userId && overview.Date == _date
                ),
                default
            );
        await unitOfWork.Received(1).SaveChangesAsync(default);
    }
}
