using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.DailyNutritionOverviews;

public class DailyNutritionOverviewQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private DailyNutritionOverviewQuery? _sut;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new DailyNutritionOverviewQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetByDateAsync_WhenOverviewExists_ShouldReturnReadModel()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var overview = DailyNutritionOverview
            .Create(DailyNutritionOverviewId.NewId(), userId, date, 100, 20, 30, 50)
            .Value;

        await WriteDbContext.DailyNutritionOverviews.AddAsync(overview);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByDateAsync(userId, date, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(overview.Id);
            result.UserId.Should().Be(userId);
            result.Date.Should().Be(date);
            result.CaloriesGoal.Should().Be(100);
            result.CarbohydratesGoal.Should().Be(20);
            result.FatGoal.Should().Be(30);
            result.ProteinGoal.Should().Be(50);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByDateAsync_WhenOverviewDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        try
        {
            // Act
            var result = await _sut!.GetByDateAsync(userId, date, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByPeriodAsync_WhenOverviewsExist_ShouldReturnReadModels()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        var overviews = new List<DailyNutritionOverview>
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 100, 20, 30, 50)
                .Value,
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    startDate.AddDays(1),
                    200,
                    40,
                    60,
                    80
                )
                .Value,
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    startDate.AddDays(2),
                    300,
                    60,
                    90,
                    120
                )
                .Value,
        };

        await WriteDbContext.DailyNutritionOverviews.AddRangeAsync(overviews);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeInAscendingOrder(x => x.Date);

            var expectedReadModels = overviews
                .Select(o => new DailyNutritionOverviewReadModel(
                    o.Id,
                    o.UserId,
                    o.Date,
                    o.CaloriesGoal,
                    o.CarbohydratesGoal,
                    o.FatGoal,
                    o.ProteinGoal
                )
                {
                    NutritionalContent = new Domain.Features.Foods.NutritionalContent
                    {
                        Energy = new Domain.Features.Foods.Energy { Unit = "Kcal", Value = 0 },
                        Calcium = 0,
                        Carbohydrates = 0,
                        Cholesterol = 0,
                        Fat = 0,
                        Fiber = 0,
                        Iron = 0,
                        MonounsaturatedFat = 0,
                        NetCarbs = 0,
                        PolyunsaturatedFat = 0,
                        Potassium = 0,
                        Protein = 0,
                        SaturatedFat = 0,
                        Sodium = 0,
                        Sugar = 0,
                        TransFat = 0,
                        VitaminA = 0,
                        VitaminC = 0,
                    },
                })
                .ToList();

            result
                .Should()
                .BeEquivalentTo(
                    expectedReadModels,
                    options =>
                        options.ComparingByMembers<DailyNutritionOverviewReadModel>().WithTracing()
                );
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByPeriodAsync_WhenNoOverviewsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByPeriodAsync_ShouldOnlyReturnOverviewsForSpecifiedUser()
    {
        // Arrange
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        var overviewsUser1 = new List<DailyNutritionOverview>
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId1, startDate, 100, 20, 30, 50)
                .Value,
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId1,
                    startDate.AddDays(1),
                    200,
                    40,
                    60,
                    80
                )
                .Value,
        };

        var overviewsUser2 = new List<DailyNutritionOverview>
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId2, startDate, 300, 60, 90, 120)
                .Value,
            DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId2,
                    startDate.AddDays(1),
                    400,
                    80,
                    120,
                    160
                )
                .Value,
        };

        await WriteDbContext.DailyNutritionOverviews.AddRangeAsync(overviewsUser1);
        await WriteDbContext.DailyNutritionOverviews.AddRangeAsync(overviewsUser2);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId1,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeInAscendingOrder(x => x.Date);

            var expectedReadModels = overviewsUser1
                .Select(o => new DailyNutritionOverviewReadModel(
                    o.Id,
                    o.UserId,
                    o.Date,
                    o.CaloriesGoal,
                    o.CarbohydratesGoal,
                    o.FatGoal,
                    o.ProteinGoal
                )
                {
                    NutritionalContent = new Domain.Features.Foods.NutritionalContent
                    {
                        Energy = new Domain.Features.Foods.Energy { Unit = "Kcal", Value = 0 },
                        Calcium = 0,
                        Carbohydrates = 0,
                        Cholesterol = 0,
                        Fat = 0,
                        Fiber = 0,
                        Iron = 0,
                        MonounsaturatedFat = 0,
                        NetCarbs = 0,
                        PolyunsaturatedFat = 0,
                        Potassium = 0,
                        Protein = 0,
                        SaturatedFat = 0,
                        Sodium = 0,
                        Sugar = 0,
                        TransFat = 0,
                        VitaminA = 0,
                        VitaminC = 0,
                    },
                })
                .ToList();

            result
                .Should()
                .BeEquivalentTo(
                    expectedReadModels,
                    options =>
                        options.ComparingByMembers<DailyNutritionOverviewReadModel>().WithTracing()
                );
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByPeriodAsync_WithEmptyDateRange_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate; // Same day

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByPeriodAsync_WithInvalidDateRange_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(-1); // End date before start date

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
