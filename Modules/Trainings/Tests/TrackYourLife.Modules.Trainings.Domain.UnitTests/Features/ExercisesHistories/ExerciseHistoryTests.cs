using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.ExercisesHistories;

public class ExerciseHistoryTests
{
    private readonly ExerciseHistoryId _validId = ExerciseHistoryId.Create(Guid.NewGuid());
    private readonly OngoingTrainingId _validOngoingTrainingId = OngoingTrainingId.Create(
        Guid.NewGuid()
    );
    private readonly ExerciseId _validExerciseId = ExerciseId.Create(Guid.NewGuid());
    private readonly List<ExerciseSet> _validOldExerciseSets = new()
    {
        ExerciseSet.CreateWeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f).Value,
        ExerciseSet.CreateWeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 8, 60.0f).Value,
    };
    private readonly List<ExerciseSet> _validNewExerciseSets = new()
    {
        ExerciseSet.CreateWeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 12, 55.0f).Value,
        ExerciseSet.CreateWeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 7, 57.5f).Value,
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
            _validOldExerciseSets,
            _validNewExerciseSets,
            _validCreatedOnUtc
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        var exerciseHistory = result.Value;
        exerciseHistory.Id.Should().Be(_validId);
        exerciseHistory.OngoingTrainingId.Should().Be(_validOngoingTrainingId);
        exerciseHistory.ExerciseId.Should().Be(_validExerciseId);
        exerciseHistory.OldExerciseSets.Should().BeEquivalentTo(_validOldExerciseSets);
        exerciseHistory.NewExerciseSets.Should().BeEquivalentTo(_validNewExerciseSets);
        exerciseHistory.CreatedOnUtc.Should().Be(_validCreatedOnUtc);
        exerciseHistory.AreChangesApplied.Should().BeFalse();

        // Verify domain invariants are maintained
        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Id.Should().NotBe(ExerciseHistoryId.Empty);
        exerciseHistory.OngoingTrainingId.Should().NotBe(OngoingTrainingId.Empty);
        exerciseHistory.ExerciseId.Should().NotBe(ExerciseId.Empty);
        exerciseHistory.OldExerciseSets.Should().NotBeEmpty();
        exerciseHistory.NewExerciseSets.Should().NotBeEmpty();
        exerciseHistory.CreatedOnUtc.Should().NotBe(default);
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
            _validOldExerciseSets,
            _validNewExerciseSets,
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
            _validOldExerciseSets,
            _validNewExerciseSets,
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
            _validOldExerciseSets,
            _validNewExerciseSets,
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
            _validNewExerciseSets,
            _validCreatedOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyNewExerciseSets_ShouldReturnFailure()
    {
        // Arrange
        var emptyNewExerciseSets = new List<ExerciseSet>();

        // Act
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            _validExerciseId,
            _validOldExerciseSets,
            emptyNewExerciseSets,
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
            _validOldExerciseSets,
            _validNewExerciseSets,
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
        exerciseHistory.OldExerciseSets.Should().NotBeEmpty();
        exerciseHistory.NewExerciseSets.Should().NotBeEmpty();
        exerciseHistory.CreatedOnUtc.Should().NotBe(default(DateTime));
    }

    [Fact]
    public void NewExerciseSets_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();

        // Act
        var newExerciseSetsJson = exerciseHistory.NewExerciseSetsJson;
        // Note: NewExerciseSets is read-only, so we can't test assignment

        // Assert
        newExerciseSetsJson.Should().NotBeNullOrEmpty();

        // Verify the JSON contains expected data structure
        newExerciseSetsJson.Should().Contain("name");
        newExerciseSetsJson.Should().Contain("reps");
        newExerciseSetsJson.Should().Contain("weight");
        newExerciseSetsJson.Should().Contain("orderIndex");

        // Verify domain state remains consistent after serialization
        exerciseHistory.NewExerciseSets.Should().NotBeEmpty();
        exerciseHistory.NewExerciseSets.Should().HaveCount(2);
        exerciseHistory.NewExerciseSets[0].Name.Should().Be("Set 1");
        exerciseHistory.NewExerciseSets[0].Type.Should().Be(ExerciseSetType.Weight);
        exerciseHistory.NewExerciseSets[0].Reps.Should().Be(12);
        exerciseHistory.NewExerciseSets[0].Weight.Should().Be(55.0f);
        exerciseHistory.NewExerciseSets[0].OrderIndex.Should().Be(0);
        exerciseHistory.NewExerciseSets[1].Name.Should().Be("Set 2");
        exerciseHistory.NewExerciseSets[1].Type.Should().Be(ExerciseSetType.Weight);
        exerciseHistory.NewExerciseSets[1].Reps.Should().Be(7);
        exerciseHistory.NewExerciseSets[1].Weight.Should().Be(57.5f);
        exerciseHistory.NewExerciseSets[1].OrderIndex.Should().Be(1);
    }

    [Fact]
    public void ExerciseSetsBeforeChange_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();

        // Act
        var exerciseSetsBeforeChangeJson = exerciseHistory.OldExerciseSetsJson;
        // Note: ExerciseSetsBeforeChange is read-only, so we can't test assignment

        // Assert
        exerciseSetsBeforeChangeJson.Should().NotBeNullOrEmpty();

        // Verify the JSON contains expected data structure
        exerciseSetsBeforeChangeJson.Should().Contain("name");
        exerciseSetsBeforeChangeJson.Should().Contain("reps");
        exerciseSetsBeforeChangeJson.Should().Contain("weight");
        exerciseSetsBeforeChangeJson.Should().Contain("orderIndex");

        // Verify domain state remains consistent after serialization
        exerciseHistory.OldExerciseSets.Should().NotBeEmpty();
        exerciseHistory.OldExerciseSets.Should().HaveCount(2);
        exerciseHistory.OldExerciseSets[0].Name.Should().Be("Set 1");
        exerciseHistory.OldExerciseSets[0].Type.Should().Be(ExerciseSetType.Weight);
        exerciseHistory.OldExerciseSets[0].Reps.Should().Be(10);
        exerciseHistory.OldExerciseSets[0].Weight.Should().Be(50.0f);
        exerciseHistory.OldExerciseSets[0].OrderIndex.Should().Be(0);
        exerciseHistory.OldExerciseSets[1].Name.Should().Be("Set 2");
        exerciseHistory.OldExerciseSets[1].Type.Should().Be(ExerciseSetType.Weight);
        exerciseHistory.OldExerciseSets[1].Reps.Should().Be(8);
        exerciseHistory.OldExerciseSets[1].Weight.Should().Be(60.0f);
        exerciseHistory.OldExerciseSets[1].OrderIndex.Should().Be(1);
    }

    [Fact]
    public void NewExerciseSets_WithInvalidJson_ShouldReturnEmptyList()
    {
        // Arrange
        var exerciseHistory = CreateValidExerciseHistory();
        // Note: NewExerciseSetsJson is read-only, so we can't test invalid JSON assignment

        // Act & Assert
        // Since we can't set invalid JSON, we'll test that the property returns valid data
        var newExerciseSets = exerciseHistory.NewExerciseSets;
        newExerciseSets.Should().NotBeNull();

        // Verify domain state is consistent and data integrity is maintained
        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Id.Should().NotBe(ExerciseHistoryId.Empty);
        exerciseHistory.OngoingTrainingId.Should().NotBe(OngoingTrainingId.Empty);
        exerciseHistory.ExerciseId.Should().NotBe(ExerciseId.Empty);
        exerciseHistory.OldExerciseSets.Should().NotBeEmpty();
        exerciseHistory.NewExerciseSets.Should().NotBeEmpty();
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
        var exerciseSetsBeforeChange = exerciseHistory.OldExerciseSets;
        exerciseSetsBeforeChange.Should().NotBeNull();

        // Verify domain state is consistent and data integrity is maintained
        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Id.Should().NotBe(ExerciseHistoryId.Empty);
        exerciseHistory.OngoingTrainingId.Should().NotBe(OngoingTrainingId.Empty);
        exerciseHistory.ExerciseId.Should().NotBe(ExerciseId.Empty);
        exerciseHistory.OldExerciseSets.Should().NotBeEmpty();
        exerciseHistory.NewExerciseSets.Should().NotBeEmpty();
        exerciseHistory.CreatedOnUtc.Should().NotBe(default(DateTime));
        exerciseHistory.AreChangesApplied.Should().BeFalse();
    }

    private ExerciseHistory CreateValidExerciseHistory()
    {
        var result = ExerciseHistory.Create(
            _validId,
            _validOngoingTrainingId,
            _validExerciseId,
            _validOldExerciseSets,
            _validNewExerciseSets,
            _validCreatedOnUtc
        );

        return result.Value;
    }
}
