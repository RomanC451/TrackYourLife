using MassTransit;
using MassTransit.Testing;
using NSubstitute;
using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Events;

public class FoodDiaryCreatedDomainEventHandlerTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private readonly UserId _userId;
    private readonly DateOnly _date;
    private readonly FoodId _foodId;
    private readonly ServingSizeId _servingSizeId;

    public FoodDiaryCreatedDomainEventHandlerTests()
    {
        _userId = UserId.NewId();
        _date = DateOnly.FromDateTime(DateTime.Today);
        _foodId = FoodId.NewId();
        _servingSizeId = ServingSizeId.NewId();
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
    public async Task Handle_WhenFoodNotFound_ShouldThrowEventFailedException()
    {
        // Arrange
        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        foodQuery.GetByIdAsync(_foodId, default).Returns((FoodReadModel?)null);

        var handler = new FoodDiaryCreatedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork,
            _bus,
            logger
        );

        var domainEvent = new FoodDiaryCreatedDomainEvent(
            _userId,
            _foodId,
            _date,
            _servingSizeId,
            1
        );

        // Act & Assert
        await Assert.ThrowsAsync<EventFailedException>(() => handler.Handle(domainEvent, default));

        await dailyNutritionOverviewRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<DailyNutritionOverview>(), default);
        await unitOfWork.DidNotReceive().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_WhenServingSizeNotFound_ShouldThrowEventFailedException()
    {
        // Arrange
        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        foodQuery
            .GetByIdAsync(_foodId, default)
            .Returns(new FoodReadModel(_foodId, "Egg", "Type", "Brand", "US"));

        servingSizeQuery.GetByIdAsync(_servingSizeId, default).Returns((ServingSizeReadModel?)null);

        var handler = new FoodDiaryCreatedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork,
            _bus,
            logger
        );

        var domainEvent = new FoodDiaryCreatedDomainEvent(
            _userId,
            _foodId,
            _date,
            _servingSizeId,
            1
        );

        // Act & Assert
        await Assert.ThrowsAsync<EventFailedException>(() => handler.Handle(domainEvent, default));

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

        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        var existingOverview = DailyNutritionOverview
            .Create(DailyNutritionOverviewId.NewId(), _userId, _date, 2000, 200, 50, 100)
            .Value;

        foodQuery
            .GetByIdAsync(_foodId, default)
            .Returns(new FoodReadModel(_foodId, "Egg", "Type", "Brand", "US"));

        servingSizeQuery
            .GetByIdAsync(_servingSizeId, default)
            .Returns(new ServingSizeReadModel(_servingSizeId, 1.0f, "g", 100, null));

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns(existingOverview);

        dailyNutritionOverviewRepository
            .WhenForAnyArgs(x => x.Update(Arg.Any<DailyNutritionOverview>()))
            .Do(x => updatedOverview = x.Arg<DailyNutritionOverview>());

        var handler = new FoodDiaryCreatedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork,
            _bus,
            logger
        );

        var domainEvent = new FoodDiaryCreatedDomainEvent(
            _userId,
            _foodId,
            _date,
            _servingSizeId,
            1
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
        updatedOverview.CaloriesGoal.Should().Be(existingOverview.CaloriesGoal);
        updatedOverview.CarbohydratesGoal.Should().Be(existingOverview.CarbohydratesGoal);
        updatedOverview.FatGoal.Should().Be(existingOverview.FatGoal);
    }

    [Fact]
    public async Task Handle_WhenOverviewDoesNotExist_ShouldCreateNewOverview()
    {
        // Arrange
        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        foodQuery
            .GetByIdAsync(_foodId, default)
            .Returns(new FoodReadModel(_foodId, "Egg", "Type", "Brand", "US"));

        servingSizeQuery
            .GetByIdAsync(_servingSizeId, default)
            .Returns(new ServingSizeReadModel(_servingSizeId, 1.0f, "g", 100, null));

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns((DailyNutritionOverview?)null);

        var handler = new FoodDiaryCreatedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork,
            _bus,
            logger
        );

        var domainEvent = new FoodDiaryCreatedDomainEvent(
            _userId,
            _foodId,
            _date,
            _servingSizeId,
            1
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        await dailyNutritionOverviewRepository
            .Received(1)
            .AddAsync(Arg.Any<DailyNutritionOverview>(), default);
        await unitOfWork.Received(1).SaveChangesAsync(default);
    }
}
