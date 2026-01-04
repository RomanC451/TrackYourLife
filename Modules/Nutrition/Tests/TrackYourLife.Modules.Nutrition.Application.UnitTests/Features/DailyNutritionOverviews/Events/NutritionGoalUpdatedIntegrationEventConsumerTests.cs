using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Events;

public class NutritionGoalUpdatedIntegrationEventConsumerTests
{
    [Fact]
    public async Task Consume_WhenNoOverviewsFound_ShouldNotUpdateAnything()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var caloriesGoal = 2000f;
        var proteinGoal = 150f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

        dailyNutritionOverviewRepository
            .GetByUserIdAndDateRangeAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<DailyNutritionOverview>());

        var provider = new ServiceCollection()
            .AddScoped(_ => dailyNutritionOverviewRepository)
            .AddScoped(_ => unitOfWork)
            .AddScoped(_ => logger)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsumer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsumer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            caloriesGoal,
            proteinGoal,
            carbohydratesGoal,
            fatGoal
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
    public async Task Consume_WhenGoalsUpdated_ShouldUpdateAllOverviews()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var caloriesGoal = 2000f;
        var proteinGoal = 150f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

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
            .AddScoped(_ => logger)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsumer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsumer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            caloriesGoal,
            proteinGoal,
            carbohydratesGoal,
            fatGoal
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
    public async Task Consume_WhenSingleOverview_ShouldUpdateAllGoals()
    {
        // Arrange
        DailyNutritionOverview updatedOverview = null!;
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var caloriesGoal = 2000f;
        var proteinGoal = 150f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

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
            .AddScoped(_ => logger)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsumer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsumer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            caloriesGoal,
            proteinGoal,
            carbohydratesGoal,
            fatGoal
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        updatedOverview.Should().NotBeNull();
        updatedOverview.CaloriesGoal.Should().Be(caloriesGoal);
        updatedOverview.ProteinGoal.Should().Be(proteinGoal);
        updatedOverview.CarbohydratesGoal.Should().Be(carbohydratesGoal);
        updatedOverview.FatGoal.Should().Be(fatGoal);
    }

    [Fact]
    public async Task Consume_WhenMultipleOverviewsWithDifferentDates_ShouldUpdateOnlyInRange()
    {
        // Arrange
        List<DailyNutritionOverview> updatedOverviews = [];

        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var caloriesGoal = 2000f;
        var proteinGoal = 150f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

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
            .AddScoped(_ => logger)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsumer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsumer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            caloriesGoal,
            proteinGoal,
            carbohydratesGoal,
            fatGoal
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
        updatedOverviews[0].CaloriesGoal.Should().Be(caloriesGoal);
        updatedOverviews[0].ProteinGoal.Should().Be(proteinGoal);
        updatedOverviews[0].CarbohydratesGoal.Should().Be(carbohydratesGoal);
        updatedOverviews[0].FatGoal.Should().Be(fatGoal);
        updatedOverviews[1].CaloriesGoal.Should().Be(caloriesGoal);
        updatedOverviews[1].ProteinGoal.Should().Be(proteinGoal);
        updatedOverviews[1].CarbohydratesGoal.Should().Be(carbohydratesGoal);
        updatedOverviews[1].FatGoal.Should().Be(fatGoal);
    }

    [Fact]
    public async Task Consume_WhenStartDateEqualsEndDate_ShouldUpdateSingleOverview()
    {
        // Arrange
        DailyNutritionOverview updatedOverview = null!;
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate; // Same date
        var caloriesGoal = 2000f;
        var proteinGoal = 150f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

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
            .AddScoped(_ => logger)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsumer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsumer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            caloriesGoal,
            proteinGoal,
            carbohydratesGoal,
            fatGoal
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
        updatedOverview.CaloriesGoal.Should().Be(caloriesGoal);
        updatedOverview.ProteinGoal.Should().Be(proteinGoal);
        updatedOverview.CarbohydratesGoal.Should().Be(carbohydratesGoal);
        updatedOverview.FatGoal.Should().Be(fatGoal);
    }

    [Fact]
    public async Task Consume_WhenZeroGoalValues_ShouldNotUpdateOverviews()
    {
        // Arrange
        DailyNutritionOverview updatedOverview = null!;
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var caloriesGoal = 0f;
        var proteinGoal = 0f;
        var carbohydratesGoal = 0f;
        var fatGoal = 0f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

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
            .AddScoped(_ => logger)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsumer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsumer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            caloriesGoal,
            proteinGoal,
            carbohydratesGoal,
            fatGoal
        );

        // Act
        await harness.Bus.Publish(@event);

        // Wait for the consumer to process the message
        Assert.True(await harness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());
        Assert.True(await consumerHarness.Consumed.Any<NutritionGoalUpdatedIntegrationEvent>());

        // Assert
        dailyNutritionOverviewRepository.Received(0).Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.Received(0).SaveChangesAsync(Arg.Any<CancellationToken>());
        updatedOverview.Should().BeNull();
    }

    [Fact]
    public async Task Consume_WhenRepositoryThrowsException_ShouldNotUpdateAnything()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        var caloriesGoal = 2000f;
        var proteinGoal = 150f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;

        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();
        var logger = Substitute.For<ILogger>();

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
            .AddScoped(_ => logger)
            .AddScoped<NutritionGoalUpdatedIntegrationEventConsumer>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<NutritionGoalUpdatedIntegrationEventConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        var consumerHarness =
            harness.GetConsumerHarness<NutritionGoalUpdatedIntegrationEventConsumer>();
        await harness.Start();

        var @event = new NutritionGoalUpdatedIntegrationEvent(
            userId,
            startDate,
            endDate,
            caloriesGoal,
            proteinGoal,
            carbohydratesGoal,
            fatGoal
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
