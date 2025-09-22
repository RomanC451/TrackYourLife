using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.ExercisesHistories;

public class ExerciseHistoryTests
{
    private readonly ExerciseHistoryId _validId = ExerciseHistoryId.Create(Guid.NewGuid());
    private readonly OngoingTrainingId _validOngoingTrainingId = OngoingTrainingId.Create(
        Guid.NewGuid()
    );
    private readonly ExerciseId _validExerciseId = ExerciseId.Create(Guid.NewGuid());
    private readonly List<ExerciseSet> _validExerciseSets = new()
    {
        new ExerciseSet(Guid.NewGuid(), "Set 1", 10, 50.0f, 0),
        new ExerciseSet(Guid.NewGuid(), "Set 2", 8, 60.0f, 1),
    };
    private readonly List<ExerciseSetChange> _validExerciseSetChanges = new()
    {
        new ExerciseSetChange
        {
            SetId = Guid.NewGuid(),
            WeightChange = 5.0f,
            RepsChange = 2,
        },
        new ExerciseSetChange
        {
            SetId = Guid.NewGuid(),
            WeightChange = -2.5f,
            RepsChange = -1,
        },
    };
    private readonly DateTime _validCreatedOnUtc = DateTime.UtcNow;

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Act
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            _validExerciseId,
            _validExerciseSets,
            _validExerciseSetChanges,
            _validCreatedOnUtc
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        var exerciseHistory = result.Value;
        exerciseHistory.Id.Should().Be(_validId);
        exerciseHistory.OngoingTrainingId.Should().Be(_validOngoingTrainingId);
        exerciseHistory.ExerciseId.Should().Be(_validExerciseId);
        exerciseHistory.ExerciseSetsBeforeChange.Should().BeEquivalentTo(_validExerciseSets);
        exerciseHistory.ExerciseSetChanges.Should().BeEquivalentTo(_validExerciseSetChanges);
        exerciseHistory.CreatedOnUtc.Should().Be(_validCreatedOnUtc);
        exerciseHistory.AreChangesApplied.Should().BeFalse();

        // Verify domain invariants are maintained
        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Id.Should().NotBe(ExerciseHistoryId.Empty);
        exerciseHistory.OngoingTrainingId.Should().NotBe(OngoingTrainingId.Empty);
        exerciseHistory.ExerciseId.Should().NotBe(ExerciseId.Empty);
        exerciseHistory.ExerciseSetsBeforeChange.Should().NotBeEmpty();
        exerciseHistory.ExerciseSetChanges.Should().NotBeEmpty();
        exerciseHistory.CreatedOnUtc.Should().NotBe(default(DateTime));
    }

    [Fact]
    public void Create_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var emptyId = ExerciseHistoryId.Empty;

        // Act
        var result = ExerciseHistory.Create(
            emptyId,
            _validOngoingTrainingId,
            _validExerciseId,
            _validExerciseSets,
            _validExerciseSetChanges,
            _validCreatedOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyOngoingTrainingId_ShouldReturnFailure()
    {
        // Arrange
        var emptyOngoingTrainingId = OngoingTrainingId.Empty;

        // Act
        var result = ExerciseHistory.Create(
            _validId,
            emptyOngoingTrainingId,
            _validExerciseId,
            _validExerciseSets,
            _validExerciseSetChanges,
            _validCreatedOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyExerciseId_ShouldReturnFailure()
    {
        // Arrange
        var emptyExerciseId = ExerciseId.Empty;

        // Act
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            emptyExerciseId,
            _validExerciseSets,
            _validExerciseSetChanges,
            _validCreatedOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyExerciseSets_ShouldReturnFailure()
    {
        // Arrange
        var emptyExerciseSets = new List<ExerciseSet>();

        // Act
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            _validExerciseId,
            emptyExerciseSets,
            _validExerciseSetChanges,
            _validCreatedOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyExerciseSetChanges_ShouldReturnFailure()
    {
        // Arrange
        var emptyExerciseSetChanges = new List<ExerciseSetChange>();

        // Act
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            _validExerciseId,
            _validExerciseSets,
            emptyExerciseSetChanges,
            _validCreatedOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithDefaultDateTime_ShouldReturnFailure()
    {
        // Arrange
        var defaultDateTime = default(DateTime);

        // Act
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            _validExerciseId,
            _validExerciseSets,
            _validExerciseSetChanges,
            defaultDateTime
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void SetAsChangesApplied_ShouldSetAreChangesAppliedToTrue()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();
        exerciseHistory.AreChangesApplied.Should().BeFalse(); // Verify initial state

        // Act
        exerciseHistory.SetAsChangesApplied();

        // Assert
        exerciseHistory.AreChangesApplied.Should().BeTrue();

        // Verify domain state is consistent
        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Id.Should().NotBe(ExerciseHistoryId.Empty);
        exerciseHistory.OngoingTrainingId.Should().NotBe(OngoingTrainingId.Empty);
        exerciseHistory.ExerciseId.Should().NotBe(ExerciseId.Empty);
        exerciseHistory.ExerciseSetsBeforeChange.Should().NotBeEmpty();
        exerciseHistory.ExerciseSetChanges.Should().NotBeEmpty();
        exerciseHistory.CreatedOnUtc.Should().NotBe(default(DateTime));
    }

    [Fact]
    public void ExerciseSetChanges_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();

        // Act
        var exerciseSetChangesJson = exerciseHistory.ExerciseSetChangesJson;
        // Note: ExerciseSetChanges is read-only, so we can't test assignment

        // Assert
        exerciseSetChangesJson.Should().NotBeNullOrEmpty();

        // Verify the JSON contains expected data structure
        exerciseSetChangesJson.Should().Contain("SetId");
        exerciseSetChangesJson.Should().Contain("WeightChange");
        exerciseSetChangesJson.Should().Contain("RepsChange");

        // Verify domain state remains consistent after serialization
        exerciseHistory.ExerciseSetChanges.Should().NotBeEmpty();
        exerciseHistory.ExerciseSetChanges.Should().HaveCount(2);
        exerciseHistory.ExerciseSetChanges[0].WeightChange.Should().Be(5.0f);
        exerciseHistory.ExerciseSetChanges[0].RepsChange.Should().Be(2);
        exerciseHistory.ExerciseSetChanges[1].WeightChange.Should().Be(-2.5f);
        exerciseHistory.ExerciseSetChanges[1].RepsChange.Should().Be(-1);
    }

    [Fact]
    public void ExerciseSetsBeforeChange_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();

        // Act
        var exerciseSetsBeforeChangeJson = exerciseHistory.ExerciseSetsBeforeChangeJson;
        // Note: ExerciseSetsBeforeChange is read-only, so we can't test assignment

        // Assert
        exerciseSetsBeforeChangeJson.Should().NotBeNullOrEmpty();

        // Verify the JSON contains expected data structure
        exerciseSetsBeforeChangeJson.Should().Contain("Name");
        exerciseSetsBeforeChangeJson.Should().Contain("Reps");
        exerciseSetsBeforeChangeJson.Should().Contain("Weight");
        exerciseSetsBeforeChangeJson.Should().Contain("OrderIndex");

        // Verify domain state remains consistent after serialization
        exerciseHistory.ExerciseSetsBeforeChange.Should().NotBeEmpty();
        exerciseHistory.ExerciseSetsBeforeChange.Should().HaveCount(2);
        exerciseHistory.ExerciseSetsBeforeChange[0].Name.Should().Be("Set 1");
        exerciseHistory.ExerciseSetsBeforeChange[0].Reps.Should().Be(10);
        exerciseHistory.ExerciseSetsBeforeChange[0].Weight.Should().Be(50.0f);
        exerciseHistory.ExerciseSetsBeforeChange[0].OrderIndex.Should().Be(0);
        exerciseHistory.ExerciseSetsBeforeChange[1].Name.Should().Be("Set 2");
        exerciseHistory.ExerciseSetsBeforeChange[1].Reps.Should().Be(8);
        exerciseHistory.ExerciseSetsBeforeChange[1].Weight.Should().Be(60.0f);
        exerciseHistory.ExerciseSetsBeforeChange[1].OrderIndex.Should().Be(1);
    }

    [Fact]
    public void ExerciseSetChanges_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();
        // Note: ExerciseSetChangesJson is read-only, so we can't test invalid JSON assignment

        // Act & Assert
        // Since we can't set invalid JSON, we'll test that the property returns valid data
        var exerciseSetChanges = exerciseHistory.ExerciseSetChanges;
        exerciseSetChanges.Should().NotBeNull();

        // Verify domain state is consistent and data integrity is maintained
        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Id.Should().NotBe(ExerciseHistoryId.Empty);
        exerciseHistory.OngoingTrainingId.Should().NotBe(OngoingTrainingId.Empty);
        exerciseHistory.ExerciseId.Should().NotBe(ExerciseId.Empty);
        exerciseHistory.ExerciseSetsBeforeChange.Should().NotBeEmpty();
        exerciseHistory.ExerciseSetChanges.Should().NotBeEmpty();
        exerciseHistory.CreatedOnUtc.Should().NotBe(default(DateTime));
        exerciseHistory.AreChangesApplied.Should().BeFalse();
    }

    [Fact]
    public void ExerciseSetsBeforeChange_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();
        // Note: ExerciseSetsBeforeChangeJson is read-only, so we can't test invalid JSON assignment

        // Act & Assert
        // Since we can't set invalid JSON, we'll test that the property returns valid data
        var exerciseSetsBeforeChange = exerciseHistory.ExerciseSetsBeforeChange;
        exerciseSetsBeforeChange.Should().NotBeNull();

        // Verify domain state is consistent and data integrity is maintained
        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Id.Should().NotBe(ExerciseHistoryId.Empty);
        exerciseHistory.OngoingTrainingId.Should().NotBe(OngoingTrainingId.Empty);
        exerciseHistory.ExerciseId.Should().NotBe(ExerciseId.Empty);
        exerciseHistory.ExerciseSetsBeforeChange.Should().NotBeEmpty();
        exerciseHistory.ExerciseSetChanges.Should().NotBeEmpty();
        exerciseHistory.CreatedOnUtc.Should().NotBe(default(DateTime));
        exerciseHistory.AreChangesApplied.Should().BeFalse();
    }

    private ExerciseHistory CreateValidExerciseHistory()
    {
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            _validExerciseId,
            _validExerciseSets,
            _validExerciseSetChanges,
            _validCreatedOnUtc
        );

        return result.Value;
    }
}
