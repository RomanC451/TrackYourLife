using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

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
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            endDate,
            OverviewType.Daily,
            AggregationMode.Sum
        );

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
    public async Task Handle_WhenOverviewsExist_WithDailyType_ShouldReturnAllDatesInRange()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(2);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            endDate,
            OverviewType.Daily,
            AggregationMode.Sum
        );

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
        result.Value.Should().BeInAscendingOrder(x => x.StartDate);
    }

    [Fact]
    public async Task Handle_WhenOverviewsExist_ShouldFillMissingDatesWithClosestOverview()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(2);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            endDate,
            OverviewType.Daily,
            AggregationMode.Sum
        );

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
        var missingDateOverview = result.Value.First(x => x.StartDate == startDate.AddDays(1));
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
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            endDate,
            OverviewType.Daily,
            AggregationMode.Sum
        );

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
        var middleDateOverview = result.Value.First(x => x.StartDate == middleDate);
        middleDateOverview.CaloriesGoal.Should().Be(overview1.CaloriesGoal); // Should use overview1 as it's closer
    }

    [Fact]
    public async Task Handle_WithWeeklyType_ShouldGroupByWeek()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        // Get start of week
        var dayOfWeek = startDate.DayOfWeek;
        var daysToSubtract = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        var weekStart = startDate.AddDays(-daysToSubtract);
        var endDate = weekStart.AddDays(13); // 2 weeks

        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            weekStart,
            endDate,
            OverviewType.Weekly,
            AggregationMode.Sum
        );

        var overviews = Enumerable
            .Range(0, 14)
            .Select(offset =>
                DailyNutritionOverviewFaker.GenerateReadModel(
                    userId: userId,
                    date: weekStart.AddDays(offset),
                    caloriesGoal: 2000,
                    carbohydratesGoal: 250,
                    fatGoal: 70,
                    proteinGoal: 150,
                    nutritionalContent: new NutritionalContent
                    {
                        Energy = new Energy { Value = 2000, Unit = "Kcal" },
                        Carbohydrates = 250,
                        Fat = 70,
                        Protein = 150,
                    }
                )
            )
            .ToList();

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, weekStart, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(overviews.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2); // 2 weeks
        result.Value.Should().BeInAscendingOrder(x => x.StartDate);
    }

    [Fact]
    public async Task Handle_WithWeeklyType_AndSumMode_ShouldSumGoals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var dayOfWeek = startDate.DayOfWeek;
        var daysToSubtract = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        var weekStart = startDate.AddDays(-daysToSubtract);
        var endDate = weekStart.AddDays(6); // 1 week

        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            weekStart,
            endDate,
            OverviewType.Weekly,
            AggregationMode.Sum
        );

        var overviews = Enumerable
            .Range(0, 7)
            .Select(offset =>
                DailyNutritionOverviewFaker.GenerateReadModel(
                    userId: userId,
                    date: weekStart.AddDays(offset),
                    caloriesGoal: 2000,
                    carbohydratesGoal: 250,
                    fatGoal: 70,
                    proteinGoal: 150
                )
            )
            .ToList();

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, weekStart, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(overviews.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var weeklyOverview = result.Value.First();
        weeklyOverview.CaloriesGoal.Should().Be(14000); // 2000 * 7
        weeklyOverview.CarbohydratesGoal.Should().Be(1750); // 250 * 7
        weeklyOverview.FatGoal.Should().Be(490); // 70 * 7
        weeklyOverview.ProteinGoal.Should().Be(1050); // 150 * 7
    }

    [Fact]
    public async Task Handle_WithWeeklyType_AndAverageMode_ShouldAverageGoals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var dayOfWeek = startDate.DayOfWeek;
        var daysToSubtract = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        var weekStart = startDate.AddDays(-daysToSubtract);
        var endDate = weekStart.AddDays(6); // 1 week

        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            weekStart,
            endDate,
            OverviewType.Weekly,
            AggregationMode.Average
        );

        var overviews = Enumerable
            .Range(0, 7)
            .Select(offset =>
                DailyNutritionOverviewFaker.GenerateReadModel(
                    userId: userId,
                    date: weekStart.AddDays(offset),
                    caloriesGoal: 2000,
                    carbohydratesGoal: 250,
                    fatGoal: 70,
                    proteinGoal: 150
                )
            )
            .ToList();

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, weekStart, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(overviews.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var weeklyOverview = result.Value.First();
        weeklyOverview.CaloriesGoal.Should().Be(2000); // Average of 2000
        weeklyOverview.CarbohydratesGoal.Should().Be(250); // Average of 250
        weeklyOverview.FatGoal.Should().Be(70); // Average of 70
        weeklyOverview.ProteinGoal.Should().Be(150); // Average of 150
    }

    [Fact]
    public async Task Handle_WithMonthlyType_ShouldGroupByMonth()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 2, 28); // 2 months

        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            endDate,
            OverviewType.Monthly,
            AggregationMode.Sum
        );

        var overviews = new List<DailyNutritionOverviewReadModel>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            overviews.Add(
                DailyNutritionOverviewFaker.GenerateReadModel(
                    userId: userId,
                    date: date,
                    caloriesGoal: 2000,
                    carbohydratesGoal: 250,
                    fatGoal: 70,
                    proteinGoal: 150
                )
            );
        }

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(overviews.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2); // 2 months
        result.Value.Should().BeInAscendingOrder(x => x.StartDate);
    }

    [Fact]
    public async Task Handle_WithMonthlyType_AndSumMode_ShouldSumGoals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 31); // 1 month

        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            endDate,
            OverviewType.Monthly,
            AggregationMode.Sum
        );

        var overviews = new List<DailyNutritionOverviewReadModel>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            overviews.Add(
                DailyNutritionOverviewFaker.GenerateReadModel(
                    userId: userId,
                    date: date,
                    caloriesGoal: 2000,
                    carbohydratesGoal: 250,
                    fatGoal: 70,
                    proteinGoal: 150
                )
            );
        }

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(overviews.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var monthlyOverview = result.Value.First();
        monthlyOverview.CaloriesGoal.Should().Be(62000); // 2000 * 31
        monthlyOverview.CarbohydratesGoal.Should().Be(7750); // 250 * 31
        monthlyOverview.FatGoal.Should().Be(2170); // 70 * 31
        monthlyOverview.ProteinGoal.Should().Be(4650); // 150 * 31
    }

    [Fact]
    public async Task Handle_WithMonthlyType_AndAverageMode_ShouldAverageGoals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 31); // 1 month

        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            endDate,
            OverviewType.Monthly,
            AggregationMode.Average
        );

        var overviews = new List<DailyNutritionOverviewReadModel>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            overviews.Add(
                DailyNutritionOverviewFaker.GenerateReadModel(
                    userId: userId,
                    date: date,
                    caloriesGoal: 2000,
                    carbohydratesGoal: 250,
                    fatGoal: 70,
                    proteinGoal: 150
                )
            );
        }

        _userIdentifierProvider.UserId.Returns(userId);
        _dailyNutritionOverviewQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(overviews.AsEnumerable()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var monthlyOverview = result.Value.First();
        monthlyOverview.CaloriesGoal.Should().Be(2000); // Average of 2000
        monthlyOverview.CarbohydratesGoal.Should().Be(250); // Average of 250
        monthlyOverview.FatGoal.Should().Be(70); // Average of 70
        monthlyOverview.ProteinGoal.Should().Be(150); // Average of 150
    }
}
