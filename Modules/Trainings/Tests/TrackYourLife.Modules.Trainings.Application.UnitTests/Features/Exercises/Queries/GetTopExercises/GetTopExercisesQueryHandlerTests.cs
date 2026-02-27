using Microsoft.Extensions.Caching.Memory;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetTopExercises;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Queries.GetTopExercises;

public class GetTopExercisesQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly IExercisesQuery _exercisesQuery;
    private readonly IMemoryCache _memoryCache;
    private readonly GetTopExercisesQueryHandler _handler;

    private readonly UserId _userId;

    public GetTopExercisesQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _memoryCache = Substitute.For<IMemoryCache>();
        _handler = new GetTopExercisesQueryHandler(
            _userIdentifierProvider,
            _exercisesHistoriesQuery,
            _exercisesQuery,
            _memoryCache
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvidedAndNoHistories_ShouldReturnEmptyPagedList()
    {
        // Arrange - use date range to avoid cache path
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _exercisesHistoriesQuery
            .GetByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetTopExercisesQuery(
            Page: 1,
            PageSize: 10,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.Page.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvidedAndHistoriesExist_ShouldReturnPagedTopExercises()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var exerciseId = ExerciseId.NewId();
        var exerciseName = "Bench Press";
        var histories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(
                exerciseId: exerciseId,
                ongoingTrainingId: OngoingTrainingId.NewId()
            ),
        };
        var exercise = ExerciseReadModelFaker.Generate(id: exerciseId, name: exerciseName);

        _exercisesHistoriesQuery
            .GetByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(histories);
        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Is<IEnumerable<ExerciseId>>(ids => ids.Contains(exerciseId)),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { exercise });

        var query = new GetTopExercisesQuery(
            Page: 1,
            PageSize: 10,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        var first = result.Value.Items.First();
        first.ExerciseId.Should().Be(exerciseId);
        first.ExerciseName.Should().Be(exerciseName);
        first.CompletionCount.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvided_ShouldUseGetByUserIdAndDateRangeAsync()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _exercisesHistoriesQuery
            .GetByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetTopExercisesQuery(
            Page: 1,
            PageSize: 10,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        await _handler.Handle(query, default);

        // Assert
        await _exercisesHistoriesQuery
            .Received(1)
            .GetByUserIdAndDateRangeAsync(
                _userId,
                startDate,
                endDate,
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenNoDateRangeProvided_ShouldUseCacheAndGetByUserIdAsync()
    {
        // Arrange - use real MemoryCache so GetOrCreateAsync runs the factory
        var realCache = new MemoryCache(new MemoryCacheOptions());
        var handlerWithRealCache = new GetTopExercisesQueryHandler(
            _userIdentifierProvider,
            _exercisesHistoriesQuery,
            _exercisesQuery,
            realCache
        );

        _exercisesHistoriesQuery
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetTopExercisesQuery(Page: 1, PageSize: 10, StartDate: null, EndDate: null);

        // Act
        var result = await handlerWithRealCache.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        await _exercisesHistoriesQuery
            .Received(1)
            .GetByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPageOutOfRange_ShouldReturnFailure()
    {
        // Arrange - one item -> maxPage=1, so Page=2 is invalid
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var exerciseId = ExerciseId.NewId();
        var histories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(exerciseId: exerciseId),
        };
        _exercisesHistoriesQuery
            .GetByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(histories);
        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Any<IEnumerable<ExerciseId>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { ExerciseReadModelFaker.Generate(id: exerciseId) });

        var query = new GetTopExercisesQuery(
            Page: 2,
            PageSize: 10,
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
