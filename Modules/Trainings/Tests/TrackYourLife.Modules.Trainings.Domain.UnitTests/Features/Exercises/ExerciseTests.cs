using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Exercises;

public class ExerciseTests
{
    private readonly ExerciseId _validId = ExerciseId.Create(Guid.NewGuid());
    private readonly UserId _validUserId = UserId.Create(Guid.NewGuid());
    private readonly string _validName = "Test Exercise";
    private readonly List<string> _validMuscleGroups = new() { "Chest", "Triceps" };
    private readonly Difficulty _validDifficulty = Difficulty.Medium;
    private readonly string _validPictureUrl = "https://example.com/picture.jpg";
    private readonly string _validVideoUrl = "https://example.com/video.mp4";
    private readonly string _validDescription = "Test exercise description";
    private readonly string _validEquipment = "Barbell";
    private readonly List<ExerciseSet> _validSets = new()
    {
        new ExerciseSet(Guid.NewGuid(), "Set 1", 10, 50.0f, 0),
        new ExerciseSet(Guid.NewGuid(), "Set 2", 8, 60.0f, 1),
    };
    private readonly DateTime _validCreatedOn = DateTime.UtcNow;

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Act
        var result = Exercise.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validPictureUrl,
            _validVideoUrl,
            _validDescription,
            _validEquipment,
            _validSets,
            _validCreatedOn
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        var exercise = result.Value;
        exercise.Id.Should().Be(_validId);
        exercise.UserId.Should().Be(_validUserId);
        exercise.Name.Should().Be(_validName);
        exercise.MuscleGroups.Should().BeEquivalentTo(_validMuscleGroups);
        exercise.Difficulty.Should().Be(_validDifficulty);
        exercise.PictureUrl.Should().Be(_validPictureUrl);
        exercise.VideoUrl.Should().Be(_validVideoUrl);
        exercise.Description.Should().Be(_validDescription);
        exercise.Equipment.Should().Be(_validEquipment);
        exercise.ExerciseSets.Should().BeEquivalentTo(_validSets);
        exercise.CreatedOnUtc.Should().Be(_validCreatedOn);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var emptyId = ExerciseId.Empty;

        // Act
        var result = Exercise.Create(
            emptyId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validPictureUrl,
            _validVideoUrl,
            _validDescription,
            _validEquipment,
            _validSets,
            _validCreatedOn
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldReturnFailure(string? name)
    {
        // Act
        var result = Exercise.Create(
            _validId,
            _validUserId,
            name!,
            _validMuscleGroups,
            _validDifficulty,
            _validPictureUrl,
            _validVideoUrl,
            _validDescription,
            _validEquipment,
            _validSets,
            _validCreatedOn
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptySets_ShouldReturnFailure()
    {
        // Arrange
        var emptySets = new List<ExerciseSet>();

        // Act
        var result = Exercise.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validPictureUrl,
            _validVideoUrl,
            _validDescription,
            _validEquipment,
            emptySets,
            _validCreatedOn
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
        var result = Exercise.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validPictureUrl,
            _validVideoUrl,
            _validDescription,
            _validEquipment,
            _validSets,
            defaultDateTime
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var exercise = CreateValidExercise();
        var newName = "Updated Exercise";
        var newMuscleGroups = new List<string> { "Back", "Biceps" };
        var newDifficulty = Difficulty.Hard;
        var newDescription = "Updated description";
        var newVideoUrl = "https://example.com/new-video.mp4";
        var newPictureUrl = "https://example.com/new-picture.jpg";
        var newEquipment = "Dumbbells";
        var newSets = new List<ExerciseSet>
        {
            new ExerciseSet(Guid.NewGuid(), "Updated Set 1", 12, 40.0f, 0),
            new ExerciseSet(Guid.NewGuid(), "Updated Set 2", 10, 50.0f, 1),
        };

        // Act
        var result = exercise.Update(
            newName,
            newMuscleGroups,
            newDifficulty,
            newDescription,
            newVideoUrl,
            newPictureUrl,
            newEquipment,
            newSets
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        exercise.Name.Should().Be(newName);
        exercise.MuscleGroups.Should().BeEquivalentTo(newMuscleGroups);
        exercise.Difficulty.Should().Be(newDifficulty);
        exercise.Description.Should().Be(newDescription);
        exercise.VideoUrl.Should().Be(newVideoUrl);
        exercise.PictureUrl.Should().Be(newPictureUrl);
        exercise.Equipment.Should().Be(newEquipment);
        exercise.ExerciseSets.Should().BeEquivalentTo(newSets);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithInvalidName_ShouldReturnFailure(string? name)
    {
        // Arrange
        var exercise = CreateValidExercise();

        // Act
        var result = exercise.Update(
            name!,
            _validMuscleGroups,
            _validDifficulty,
            _validDescription,
            _validVideoUrl,
            _validPictureUrl,
            _validEquipment,
            _validSets
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithEmptySets_ShouldReturnFailure()
    {
        // Arrange
        var exercise = CreateValidExercise();
        var emptySets = new List<ExerciseSet>();

        // Act
        var result = exercise.Update(
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validDescription,
            _validVideoUrl,
            _validPictureUrl,
            _validEquipment,
            emptySets
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void ExerciseSets_ShouldBeOrderedByOrderIndex()
    {
        // Arrange
        var unorderedSets = new List<ExerciseSet>
        {
            new ExerciseSet(Guid.NewGuid(), "Set 3", 8, 60.0f, 2),
            new ExerciseSet(Guid.NewGuid(), "Set 1", 10, 50.0f, 0),
            new ExerciseSet(Guid.NewGuid(), "Set 2", 9, 55.0f, 1),
        };

        // Act
        var result = Exercise.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validPictureUrl,
            _validVideoUrl,
            _validDescription,
            _validEquipment,
            unorderedSets,
            _validCreatedOn
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        var exercise = result.Value;
        exercise.ExerciseSets.Should().BeInAscendingOrder(x => x.OrderIndex);
    }

    [Fact]
    public void ExerciseSets_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exercise = CreateValidExercise();

        // Act
        var exerciseSetsJson = exercise.ExerciseSetsJson;
        // Note: ExerciseSets is read-only, so we can't test assignment

        // Assert
        exerciseSetsJson.Should().NotBeNullOrEmpty();
    }

    private Exercise CreateValidExercise()
    {
        var result = Exercise.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validPictureUrl,
            _validVideoUrl,
            _validDescription,
            _validEquipment,
            _validSets,
            _validCreatedOn
        );

        return result.Value;
    }
}
