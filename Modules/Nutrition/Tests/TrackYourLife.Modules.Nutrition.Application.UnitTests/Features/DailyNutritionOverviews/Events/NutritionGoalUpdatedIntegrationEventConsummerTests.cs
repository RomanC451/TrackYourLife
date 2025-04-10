using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Events;

public class NutritionGoalUpdatedIntegrationEventConsummerTests //: IAsyncLifetime
{
    [Fact]
    public async Task Consume_WhenNoOverviewsFound_ShouldNotUpdateAnything()
    {
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<DailyNutritionOverview>());

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        // Arrange

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Calories
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

        dailyNutritionOverviewRepository.ClearSubstitute();
    }

    [Fact]
    public async Task Consume_WhenCaloriesGoalUpdated_ShouldUpdateAllOverviews()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        IEnumerable<DailyNutritionOverview> overviews = new[]
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value,
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    startDate.AddDays(1),
                    1900,
                    210,
                    55,
                    110
                )
                .Value,
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews);

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Calories
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(2).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenCarbohydratesGoalUpdated_ShouldUpdateAllOverviews()
    {
        // Arrange

        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var overviews = new[]
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value,
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews);

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Carbohydrates
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenFatsGoalUpdated_ShouldUpdateAllOverviews()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var overviews = new[]
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value,
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews);

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Fats
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenProteinGoalUpdated_ShouldUpdateAllOverviews()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var overviews = new[]
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value,
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews);

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Protein
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenInvalidGoalType_ShouldNotUpdateAnything()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var overviews = new[]
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value,
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews);

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            (GoalType)999 // Invalid goal type
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenMultipleOverviewsWithDifferentDates_ShouldUpdateOnlyInRange()
    {
        // Arrange
        List<DailyNutritionOverview> updatedOverviews = [];

        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        IEnumerable<DailyNutritionOverview> overviews = new[]
        {
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    startDate.AddDays(-1),
                    1800,
                    200,
                    50,
                    100
                )
                .Value, // Before range
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value, // In range
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    startDate.AddDays(3),
                    1900,
                    210,
                    55,
                    110
                )
                .Value, // In range
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    endDate.AddDays(1),
                    1900,
                    210,
                    55,
                    110
                )
                .Value, // After range
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews.Where(o => o.Date >= startDate && o.Date <= endDate));

        dailyNutritionOverviewRepository
            .WhenForAnyArgs(x => x.Update(Arg.Any<DailyNutritionOverview>()))
            .Do(x => updatedOverviews.Add(x.Arg<DailyNutritionOverview>()));

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Calories
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(2).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        updatedOverviews.Should().HaveCount(2);
        updatedOverviews[0].CaloriesGoal.Should().Be(newGoalValue);
        updatedOverviews[1].CaloriesGoal.Should().Be(newGoalValue);
    }

    [Fact]
    public async Task Consume_WhenStartDateEqualsEndDate_ShouldUpdateSingleOverview()
    {
        // Arrange
        DailyNutritionOverview updatedOverview = null!;
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate; // Same date
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var overviews = new[]
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value,
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews);

        dailyNutritionOverviewRepository
            .WhenForAnyArgs(x => x.Update(Arg.Any<DailyNutritionOverview>()))
            .Do(x => updatedOverview = x.Arg<DailyNutritionOverview>());

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Calories
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(updatedOverview);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        updatedOverview.Should().NotBeNull();
        updatedOverview.Id.Should().Be(overviews[0].Id);
        updatedOverview.UserId.Should().Be(overviews[0].UserId);
        updatedOverview.Date.Should().Be(overviews[0].Date);
        updatedOverview.CaloriesGoal.Should().Be(newGoalValue);
    }

    [Fact]
    public async Task Consume_WhenZeroGoalValue_ShouldNotUpdateOverviews()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 0f; // Zero goal value

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var overviews = new[]
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 1800, 200, 50, 100)
                .Value,
        };

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(overviews);

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Calories
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(0).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(0).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenRepositoryThrowsException_ShouldNotUpdateAnything()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var newGoalValue = 2000f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromException<IEnumerable<DailyNutritionOverview>>(
                    new Exception("Repository error")
                )
            );

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsummer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsummer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            newGoalValue,
            GoalType.Calories
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
