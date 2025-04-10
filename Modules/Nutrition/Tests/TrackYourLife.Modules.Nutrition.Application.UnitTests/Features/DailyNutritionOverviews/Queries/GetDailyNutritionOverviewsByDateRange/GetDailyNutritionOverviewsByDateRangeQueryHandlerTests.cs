using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

public class GetDailyNutritionOverviewsByDateRangeQueryHandlerTests
{
    private readonly IDailyNutritionOverviewQuery _dailyNutritionOverviewQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetDailyNutritionOverviewsByDateRangeQueryHandler _handler;

    public GetDailyNutritionOverviewsByDateRangeQueryHandlerTests()
    {
        _dailyNutritionOverviewQuery = Substitute.For<IDailyNutritionOverviewQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetDailyNutritionOverviewsByDateRangeQueryHandler(
            _dailyNutritionOverviewQuery,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenNoOverviewsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(2);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(startDate, endDate);

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Enumerable.Empty<DailyNutritionOverviewReadModel>()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenOverviewsExist_ShouldReturnAllDatesInRange()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(2);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(startDate, endDate);

        var existingOverview = DailyNutritionOverviewFaker.GenerateReadModel(
            userId: userId,
            date: startDate
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new[] { existingOverview }.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3); // Should include all dates in range
        result.Value.Should().BeInAscendingOrder(x => x.Date);
    }

    [Fact]
    public async Task Handle_WhenOverviewsExist_ShouldFillMissingDatesWithClosestOverview()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(2);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(startDate, endDate);

        var existingOverview = DailyNutritionOverviewFaker.GenerateReadModel(
            userId: userId,
            date: startDate,
            caloriesGoal: 2000,
            carbohydratesGoal: 250,
            fatGoal: 70,
            proteinGoal: 150
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new[] { existingOverview }.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        // Verify that missing dates are filled with the closest overview's goals
        var missingDateOverview = result.Value.First(x => x.Date == startDate.AddDays(1));
        missingDateOverview.CaloriesGoal.Should().Be(existingOverview.CaloriesGoal);
        missingDateOverview.CarbohydratesGoal.Should().Be(existingOverview.CarbohydratesGoal);
        missingDateOverview.FatGoal.Should().Be(existingOverview.FatGoal);
        missingDateOverview.ProteinGoal.Should().Be(existingOverview.ProteinGoal);
    }

    [Fact]
    public async Task Handle_WhenMultipleOverviewsExist_ShouldUseClosestOverviewForMissingDates()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(3);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(startDate, endDate);

        var overview1 = DailyNutritionOverviewFaker.GenerateReadModel(
            userId: userId,
            date: startDate,
            caloriesGoal: 2000,
            carbohydratesGoal: 250,
            fatGoal: 70,
            proteinGoal: 150
        );

        var overview2 = DailyNutritionOverviewFaker.GenerateReadModel(
            userId: userId,
            date: endDate,
            caloriesGoal: 1800,
            carbohydratesGoal: 200,
            fatGoal: 60,
            proteinGoal: 120
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new[] { overview1, overview2 }.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(4);

        // Verify that the middle date uses the closest overview's goals
        var middleDate = startDate.AddDays(1);
        var middleDateOverview = result.Value.First(x => x.Date == middleDate);
        middleDateOverview.CaloriesGoal.Should().Be(overview1.CaloriesGoal); // Should use overview1 as it's closer
    }
}
