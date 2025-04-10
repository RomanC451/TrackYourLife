using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.DailyNutritionOverviews;

[Collection("NutritionRepositoryTests")]
public class DailyNutritionOverviewRepositoryTests : BaseRepositoryTests
{
    private DailyNutritionOverviewRepository? _sut;
    private DailyNutritionOverviewQuery? _query;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new DailyNutritionOverviewRepository(_writeDbContext!);
        _query = new DailyNutritionOverviewQuery(_readDbContext!);
    }

    [Fact]
    public async Task GetByUserIdAndDateAsync_WhenOverviewExists_ShouldReturnOverview()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var overview = DailyNutritionOverview
            .Create(DailyNutritionOverviewId.NewId(), userId, date, 0, 0, 0, 0)
            .Value;

        await _writeDbContext!.DailyNutritionOverviews.AddAsync(overview);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateAsync(userId, date, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(overview.Id);
            result.UserId.Should().Be(userId);
            result.Date.Should().Be(date);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndDateAsync_WhenOverviewDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateAsync(userId, date, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndDateRangeAsync_WhenOverviewsExist_ShouldReturnOverviews()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        var overviews = new List<DailyNutritionOverview>
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate, 0, 0, 0, 0)
                .Value,
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate.AddDays(1), 0, 0, 0, 0)
                .Value,
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId, startDate.AddDays(2), 0, 0, 0, 0)
                .Value,
        };

        await _writeDbContext!.DailyNutritionOverviews.AddRangeAsync(overviews);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateRangeAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(overviews);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndDateRangeAsync_WhenNoOverviewsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateRangeAsync(
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
    public async Task GetByUserIdAndDateRangeAsync_ShouldOnlyReturnOverviewsForSpecifiedUser()
    {
        // Arrange
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        var overviewsUser1 = new List<DailyNutritionOverview>
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId1, startDate, 0, 0, 0, 0)
                .Value,
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId1, startDate.AddDays(1), 0, 0, 0, 0)
                .Value,
        };

        var overviewsUser2 = new List<DailyNutritionOverview>
        {
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId2, startDate, 0, 0, 0, 0)
                .Value,
            DailyNutritionOverview
                .Create(DailyNutritionOverviewId.NewId(), userId2, startDate.AddDays(1), 0, 0, 0, 0)
                .Value,
        };

        await _writeDbContext!.DailyNutritionOverviews.AddRangeAsync(overviewsUser1);
        await _writeDbContext.DailyNutritionOverviews.AddRangeAsync(overviewsUser2);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateRangeAsync(
                userId1,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(overviewsUser1);
            result.Should().NotContain(overviewsUser2);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndDateRangeAsync_WithEmptyDateRange_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate; // Same day

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateRangeAsync(
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
    public async Task GetByUserIdAndDateRangeAsync_WithInvalidDateRange_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(-1); // End date before start date

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateRangeAsync(
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
    public async Task GetByUserIdAndDateRangeAsync_WithLargeDateRange_ShouldReturnAllOverviewsInRange()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(30);

        // Create test data with unique dates
        var overviews = new List<DailyNutritionOverview>();
        for (int i = 0; i <= 30; i++) // Changed to <= 30 to include the end date
        {
            var date = startDate.AddDays(i);
            var overview = DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    date,
                    100, // caloriesGoal
                    20, // carbohydratesGoal
                    30, // fatGoal
                    50 // proteinGoal
                )
                .Value;
            overviews.Add(overview);
        }

        foreach (var overview in overviews)
        {
            await _sut!.AddAsync(overview, CancellationToken.None);
        }
        await _writeDbContext!.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByUserIdAndDateRangeAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(31); // 31 days (inclusive of start and end dates)
            result.Should().BeEquivalentTo(overviews);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndDateRangeAsync_WhenCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(7);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        try
        {
            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _sut!.GetByUserIdAndDateRangeAsync(userId, startDate, endDate, cts.Token)
            );
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndDateAsync_WhenCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        try
        {
            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => _sut!.GetByUserIdAndDateAsync(userId, date, cts.Token)
            );
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndDateRangeAsync_WithConcurrentOperations_ShouldHandleCorrectly()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(7);

        // Create test data
        var overviews = new List<DailyNutritionOverview>();
        for (int i = 0; i <= 7; i++) // Changed to <= 7 to include the end date
        {
            var date = startDate.AddDays(i);
            var overview = DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userId,
                    date,
                    100, // caloriesGoal
                    20, // carbohydratesGoal
                    30, // fatGoal
                    50 // proteinGoal
                )
                .Value;
            overviews.Add(overview);
        }

        // Add all overviews to the database
        foreach (var overview in overviews)
        {
            await _sut!.AddAsync(overview, CancellationToken.None);
        }
        await _writeDbContext!.SaveChangesAsync();

        // Create a list to store all contexts
        var contexts = new List<NutritionWriteDbContext>();

        try
        {
            // Act - Perform concurrent operations
            var tasks = new List<Task<IEnumerable<DailyNutritionOverview>>>();
            for (int i = 0; i < 5; i++)
            {
                var context = new NutritionWriteDbContext(
                    new DbContextOptionsBuilder<NutritionWriteDbContext>()
                        .UseNpgsql(_dbContainer.GetConnectionString())
                        .Options,
                    null
                );
                contexts.Add(context);
                var repository = new DailyNutritionOverviewRepository(context);
                tasks.Add(
                    repository.GetByUserIdAndDateRangeAsync(
                        userId,
                        startDate,
                        endDate,
                        CancellationToken.None
                    )
                );
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            foreach (var result in results)
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(8); // 8 days (inclusive of start and end dates)
                result.Should().BeEquivalentTo(overviews);
            }
        }
        finally
        {
            // Clean up contexts
            foreach (var context in contexts)
            {
                await context.DisposeAsync();
            }
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task Query_GetByDateAsync_ShouldReturnReadModel()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var overview = DailyNutritionOverview
            .Create(DailyNutritionOverviewId.NewId(), userId, date, 100, 20, 30, 50)
            .Value;

        await _writeDbContext!.DailyNutritionOverviews.AddAsync(overview);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _query!.GetByDateAsync(userId, date, CancellationToken.None);

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
    public async Task Query_GetByPeriodAsync_ShouldReturnReadModels()
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

        await _writeDbContext!.DailyNutritionOverviews.AddRangeAsync(overviews);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _query!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

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
}
