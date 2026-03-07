using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetMuscleGroupDistribution;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetMuscleGroupDistribution;

public class GetMuscleGroupDistributionQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly IExercisesQuery _exercisesQuery;
    private readonly IMuscleGroupsQuery _muscleGroupsQuery;
    private readonly GetMuscleGroupDistributionQueryHandler _handler;

    private readonly UserId _userId;

    public GetMuscleGroupDistributionQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _muscleGroupsQuery = Substitute.For<IMuscleGroupsQuery>();
        _muscleGroupsQuery.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<MuscleGroup>());

        _handler = new GetMuscleGroupDistributionQueryHandler(
            _userIdentifierProvider,
            _exercisesHistoriesQuery,
            _exercisesQuery,
            _muscleGroupsQuery
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoCompletedHistories_ShouldReturnEmptyList()
    {
        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetMuscleGroupDistributionQuery(
            StartDate: null,
            EndDate: null,
            ParentMuscleGroupName: null
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCompletedHistoriesExist_ShouldReturnMuscleGroupDistribution()
    {
        var chestTricepsId = ExerciseId.NewId();
        var backId = ExerciseId.NewId();
        var exerciseChestTriceps = ExerciseReadModelFaker.Generate(
            id: chestTricepsId,
            muscleGroups: new List<string> { "Chest", "Triceps" }
        );
        var exerciseBack = ExerciseReadModelFaker.Generate(
            id: backId,
            muscleGroups: new List<string> { "Back" }
        );

        var history1 = ExerciseHistoryReadModelFaker.Generate(exerciseId: chestTricepsId);
        var history2 = ExerciseHistoryReadModelFaker.Generate(exerciseId: backId);

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { history1, history2 });

        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Is<IEnumerable<ExerciseId>>(ids => ids.Count() == 2),
                Arg.Any<CancellationToken>()
            )
            .Returns(new[] { exerciseChestTriceps, exerciseBack });

        var query = new GetMuscleGroupDistributionQuery(
            StartDate: null,
            EndDate: null,
            ParentMuscleGroupName: null
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Select(d => d.MuscleGroup).Should().Contain("Chest");
        result.Value.Select(d => d.MuscleGroup).Should().Contain("Triceps");
        result.Value.Select(d => d.MuscleGroup).Should().Contain("Back");
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetMuscleGroupDistributionQuery(
            StartDate: null,
            EndDate: null,
            ParentMuscleGroupName: null
        );

        await _handler.Handle(query, default);

        await _exercisesHistoriesQuery
            .Received(1)
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDateRangeProvided_ShouldCallGetCompletedByUserIdAndDateRangeAsync()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _exercisesHistoriesQuery
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<ExerciseHistoryReadModel>());

        var query = new GetMuscleGroupDistributionQuery(
            StartDate: startDate,
            EndDate: endDate,
            ParentMuscleGroupName: null
        );

        await _handler.Handle(query, default);

        await _exercisesHistoriesQuery
            .Received(1)
            .GetCompletedByUserIdAndDateRangeAsync(
                _userId,
                startDate,
                endDate,
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenExerciseNotFoundForHistory_ShouldSkipThatHistory()
    {
        var unknownExerciseId = ExerciseId.NewId();
        var history = ExerciseHistoryReadModelFaker.Generate(exerciseId: unknownExerciseId);

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { history });

        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(Arg.Any<IEnumerable<ExerciseId>>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseReadModel>());

        var query = new GetMuscleGroupDistributionQuery(
            StartDate: null,
            EndDate: null,
            ParentMuscleGroupName: null
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenNoParentName_ShouldAggregateByMainMuscleGroupOnly()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var upperChestId = new MuscleGroupId(Guid.NewGuid());
        var tricepsId = new MuscleGroupId(Guid.NewGuid());
        var muscleGroups = new List<MuscleGroup>
        {
            MuscleGroup.Create(chestId, "Chest", null),
            MuscleGroup.Create(upperChestId, "Upper Chest", chestId),
            MuscleGroup.Create(tricepsId, "Triceps", null)
        };
        _muscleGroupsQuery.GetAllAsync(Arg.Any<CancellationToken>()).Returns(muscleGroups);

        var upperChestExerciseId = ExerciseId.NewId();
        var chestExerciseId = ExerciseId.NewId();
        var exerciseUpperChestTriceps = ExerciseReadModelFaker.Generate(
            id: upperChestExerciseId,
            muscleGroups: new List<string> { "Upper Chest", "Triceps" }
        );
        var exerciseChest = ExerciseReadModelFaker.Generate(
            id: chestExerciseId,
            muscleGroups: new List<string> { "Chest" }
        );

        // Use explicit set counts so WorkoutCount (weight = set count per muscle group) is deterministic
        var twoSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 8, "reps", 60.0f, "kg").Value
        };
        var threeSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 8, "reps", 60.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 3", 2, 6, "reps", 70.0f, "kg").Value
        };
        var history1 = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: upperChestExerciseId,
            newExerciseSets: twoSets
        );
        var history2 = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: chestExerciseId,
            newExerciseSets: threeSets
        );

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { history1, history2 });

        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(Arg.Any<IEnumerable<ExerciseId>>(), Arg.Any<CancellationToken>())
            .Returns(new[] { exerciseUpperChestTriceps, exerciseChest });

        var query = new GetMuscleGroupDistributionQuery(
            StartDate: null,
            EndDate: null,
            ParentMuscleGroupName: null
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(d => d.MuscleGroup).Should().Contain("Chest");
        result.Value.Select(d => d.MuscleGroup).Should().Contain("Triceps");
        result.Value.Should().HaveCount(2);
        // WorkoutCount = sum of set counts: history1 (2 sets) contributes to Chest and Triceps; history2 (3 sets) to Chest only
        var chestDto = result.Value.Single(d => d.MuscleGroup == "Chest");
        chestDto.WorkoutCount.Should().Be(5); // 2 (Upper Chest) + 3 (Chest)
        var tricepsDto = result.Value.Single(d => d.MuscleGroup == "Triceps");
        tricepsDto.WorkoutCount.Should().Be(2); // 2 sets from history1
    }

    [Fact]
    public async Task Handle_WhenParentMuscleGroupNameProvided_ShouldReturnOnlySubgroupsOfThatParent()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var upperChestId = new MuscleGroupId(Guid.NewGuid());
        var lowerChestId = new MuscleGroupId(Guid.NewGuid());
        var muscleGroups = new List<MuscleGroup>
        {
            MuscleGroup.Create(chestId, "Chest", null),
            MuscleGroup.Create(upperChestId, "Upper Chest", chestId),
            MuscleGroup.Create(lowerChestId, "Lower Chest", chestId)
        };
        _muscleGroupsQuery.GetAllAsync(Arg.Any<CancellationToken>()).Returns(muscleGroups);

        var upperLowerId = ExerciseId.NewId();
        var chestUpperId = ExerciseId.NewId();
        var exerciseUpperLower = ExerciseReadModelFaker.Generate(
            id: upperLowerId,
            muscleGroups: new List<string> { "Upper Chest", "Lower Chest" }
        );
        var exerciseChestUpper = ExerciseReadModelFaker.Generate(
            id: chestUpperId,
            muscleGroups: new List<string> { "Chest", "Upper Chest" }
        );

        // Use explicit set counts so WorkoutCount (weight = set count per muscle group) is deterministic
        var twoSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 8, "reps", 60.0f, "kg").Value
        };
        var threeSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 8, "reps", 60.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 3", 2, 6, "reps", 70.0f, "kg").Value
        };
        var history1 = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: upperLowerId,
            newExerciseSets: twoSets
        );
        var history2 = ExerciseHistoryReadModelFaker.Generate(
            exerciseId: chestUpperId,
            newExerciseSets: threeSets
        );

        _exercisesHistoriesQuery
            .GetCompletedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new[] { history1, history2 });

        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(Arg.Any<IEnumerable<ExerciseId>>(), Arg.Any<CancellationToken>())
            .Returns(new[] { exerciseUpperLower, exerciseChestUpper });

        var query = new GetMuscleGroupDistributionQuery(
            StartDate: null,
            EndDate: null,
            ParentMuscleGroupName: "Chest"
        );

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(d => d.MuscleGroup).Should().Contain("Upper Chest");
        result.Value.Select(d => d.MuscleGroup).Should().Contain("Lower Chest");
        result.Value.Should().NotContain(d => d.MuscleGroup == "Chest");
        result.Value.Should().HaveCount(2);
        // WorkoutCount = sum of set counts: Upper Chest in both (2 + 3), Lower Chest only in history1 (2)
        result.Value.Single(d => d.MuscleGroup == "Upper Chest").WorkoutCount.Should().Be(5);
        result.Value.Single(d => d.MuscleGroup == "Lower Chest").WorkoutCount.Should().Be(2);
    }
}
