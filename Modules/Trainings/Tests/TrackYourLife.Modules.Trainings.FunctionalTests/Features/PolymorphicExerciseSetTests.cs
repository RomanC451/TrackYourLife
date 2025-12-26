using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using Xunit;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Features;

public class PolymorphicExerciseSetTests
{
    [Fact]
    public void Exercise_WithMixedExerciseSets_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exerciseSets = new List<ExerciseSet>
        {
            new WeightBasedExerciseSet(
                Guid.NewGuid(),
                "Bench Press",
                1,
                10,
                50.5f,
                TimeSpan.FromMinutes(2)
            ),
            new TimeBasedExerciseSet(
                Guid.NewGuid(),
                "Plank",
                2,
                TimeSpan.FromSeconds(60),
                TimeSpan.FromMinutes(1)
            ),
            new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 3, 15, TimeSpan.FromMinutes(1)),
            new DistanceExerciseSet(
                Guid.NewGuid(),
                "5K Run",
                4,
                5.0f,
                "km",
                TimeSpan.FromMinutes(5)
            ),
            new CustomExerciseSet(
                Guid.NewGuid(),
                "Rope Climbing",
                5,
                "3",
                "ascents",
                TimeSpan.FromMinutes(3)
            ),
        };

        var exerciseResult = Exercise.Create(
            ExerciseId.NewId(),
            UserId.NewId(),
            "Mixed Workout",
            new List<string> { "Full Body" },
            Difficulty.Medium,
            null,
            null,
            "A workout with different types of exercises",
            "None",
            exerciseSets,
            DateTime.UtcNow
        );

        exerciseResult.IsSuccess.Should().BeTrue();
        var exercise = exerciseResult.Value;

        // Act
        var json = exercise.ExerciseSetsJson;
        var deserializedSets = ExerciseSetJsonSerializer.Deserialize(json);

        // Assert
        deserializedSets.Should().HaveCount(5);
        deserializedSets[0].Should().BeOfType<WeightBasedExerciseSet>();
        deserializedSets[1].Should().BeOfType<TimeBasedExerciseSet>();
        deserializedSets[2].Should().BeOfType<BodyweightExerciseSet>();
        deserializedSets[3].Should().BeOfType<DistanceExerciseSet>();
        deserializedSets[4].Should().BeOfType<CustomExerciseSet>();

        // Verify all properties are preserved
        var weightSet = (WeightBasedExerciseSet)deserializedSets[0];
        weightSet.Name.Should().Be("Bench Press");
        weightSet.Reps.Should().Be(10);
        weightSet.Weight.Should().Be(50.5f);

        var timeSet = (TimeBasedExerciseSet)deserializedSets[1];
        timeSet.Name.Should().Be("Plank");
        timeSet.Duration.Should().Be(TimeSpan.FromSeconds(60));

        var bodyweightSet = (BodyweightExerciseSet)deserializedSets[2];
        bodyweightSet.Name.Should().Be("Push-ups");
        bodyweightSet.Reps.Should().Be(15);

        var distanceSet = (DistanceExerciseSet)deserializedSets[3];
        distanceSet.Name.Should().Be("5K Run");
        distanceSet.Distance.Should().Be(5.0f);
        distanceSet.DistanceUnit.Should().Be("km");

        var customSet = (CustomExerciseSet)deserializedSets[4];
        customSet.Name.Should().Be("Rope Climbing");
        customSet.CustomValue.Should().Be("3");
        customSet.CustomUnit.Should().Be("ascents");
    }

    [Fact]
    public void ExerciseHistory_WithMixedExerciseSetChanges_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exerciseSetChanges = new List<ExerciseSetChange>
        {
            new WeightBasedExerciseSetChange(Guid.NewGuid(), 5.0f, 2),
            new TimeBasedExerciseSetChange(Guid.NewGuid(), TimeSpan.FromSeconds(30)),
            new BodyweightExerciseSetChange(Guid.NewGuid(), 3),
            new DistanceExerciseSetChange(Guid.NewGuid(), 1.5f),
            new CustomExerciseSetChange(Guid.NewGuid(), "2"),
        };

        var exerciseSetsBeforeChange = new List<ExerciseSet>
        {
            new WeightBasedExerciseSet(Guid.NewGuid(), "Bench Press", 1, 8, 45.0f),
            new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 2, TimeSpan.FromSeconds(45)),
            new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 3, 12),
            new DistanceExerciseSet(Guid.NewGuid(), "3K Run", 4, 3.0f, "km"),
            new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 5, "1", "ascents"),
        };

        var exerciseHistoryResult = ExerciseHistory.Create(
            ExerciseHistoryId.NewId(),
            OngoingTrainingId.NewId(),
            ExerciseId.NewId(),
            exerciseSetsBeforeChange,
            exerciseSetChanges,
            DateTime.UtcNow
        );

        exerciseHistoryResult.IsSuccess.Should().BeTrue();
        var exerciseHistory = exerciseHistoryResult.Value;

        // Act
        var updatedExerciseHistoryResult = ExerciseHistory.Create(
            ExerciseHistoryId.NewId(),
            exerciseHistory.OngoingTrainingId,
            exerciseHistory.ExerciseId,
            exerciseSetsBeforeChange,
            exerciseSetChanges,
            exerciseHistory.CreatedOnUtc
        );

        updatedExerciseHistoryResult.IsSuccess.Should().BeTrue();
        var updatedExerciseHistory = updatedExerciseHistoryResult.Value;

        var changesJson = updatedExerciseHistory.NewExerciseSetsJson;
        var setsJson = updatedExerciseHistory.OldExerciseSetsJson;

        var deserializedChanges = ExerciseSetChangeJsonSerializer.Deserialize(changesJson);
        var deserializedSets = ExerciseSetJsonSerializer.Deserialize(setsJson);

        // Assert
        deserializedChanges.Should().HaveCount(5);
        deserializedChanges[0].Should().BeOfType<WeightBasedExerciseSetChange>();
        deserializedChanges[1].Should().BeOfType<TimeBasedExerciseSetChange>();
        deserializedChanges[2].Should().BeOfType<BodyweightExerciseSetChange>();
        deserializedChanges[3].Should().BeOfType<DistanceExerciseSetChange>();
        deserializedChanges[4].Should().BeOfType<CustomExerciseSetChange>();

        deserializedSets.Should().HaveCount(5);
        deserializedSets[0].Should().BeOfType<WeightBasedExerciseSet>();
        deserializedSets[1].Should().BeOfType<TimeBasedExerciseSet>();
        deserializedSets[2].Should().BeOfType<BodyweightExerciseSet>();
        deserializedSets[3].Should().BeOfType<DistanceExerciseSet>();
        deserializedSets[4].Should().BeOfType<CustomExerciseSet>();
    }

    [Fact]
    public void ExerciseReadModel_WithMixedExerciseSets_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var exerciseReadModel = new ExerciseReadModel(
            ExerciseId.NewId(),
            UserId.NewId(),
            "Mixed Workout",
            new List<string> { "Full Body" },
            Difficulty.Medium,
            null,
            null,
            "A workout with different types of exercises",
            "None",
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        var exerciseSets = new List<ExerciseSet>
        {
            new WeightBasedExerciseSet(Guid.NewGuid(), "Bench Press", 1, 10, 50.5f),
            new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 2, TimeSpan.FromSeconds(60)),
            new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 3, 15),
            new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 4, 5.0f, "km"),
            new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 5, "3", "ascents"),
        };

        // Act
        var updatedExerciseReadModel = exerciseReadModel with
        {
            ExerciseSets = exerciseSets,
        };
        var json = updatedExerciseReadModel.ExerciseSetsJson;
        var deserializedSets = ExerciseSetJsonSerializer.Deserialize(json);

        // Assert
        deserializedSets.Should().HaveCount(5);
        deserializedSets[0].Should().BeOfType<WeightBasedExerciseSet>();
        deserializedSets[1].Should().BeOfType<TimeBasedExerciseSet>();
        deserializedSets[2].Should().BeOfType<BodyweightExerciseSet>();
        deserializedSets[3].Should().BeOfType<DistanceExerciseSet>();
        deserializedSets[4].Should().BeOfType<CustomExerciseSet>();
    }

    [Fact]
    public void DTO_Mapping_WithMixedExerciseSets_ShouldMapCorrectly()
    {
        // Arrange
        var exerciseSets = new List<ExerciseSet>
        {
            new WeightBasedExerciseSet(Guid.NewGuid(), "Bench Press", 1, 10, 50.5f),
            new TimeBasedExerciseSet(Guid.NewGuid(), "Plank", 2, TimeSpan.FromSeconds(60)),
            new BodyweightExerciseSet(Guid.NewGuid(), "Push-ups", 3, 15),
            new DistanceExerciseSet(Guid.NewGuid(), "5K Run", 4, 5.0f, "km"),
            new CustomExerciseSet(Guid.NewGuid(), "Rope Climbing", 5, "3", "ascents"),
        };

        // Act
        var dtos = exerciseSets.Select(es => es.ToDto()).ToList();
        var mappedBack = dtos.Select(dto => dto.ToEntity()).ToList();

        // Assert
        mappedBack.Should().HaveCount(5);
        mappedBack[0].Should().BeOfType<WeightBasedExerciseSet>();
        mappedBack[1].Should().BeOfType<TimeBasedExerciseSet>();
        mappedBack[2].Should().BeOfType<BodyweightExerciseSet>();
        mappedBack[3].Should().BeOfType<DistanceExerciseSet>();
        mappedBack[4].Should().BeOfType<CustomExerciseSet>();

        // Verify properties are preserved through the mapping
        var weightSet = (WeightBasedExerciseSet)mappedBack[0];
        weightSet.Name.Should().Be("Bench Press");
        weightSet.Reps.Should().Be(10);
        weightSet.Weight.Should().Be(50.5f);

        var timeSet = (TimeBasedExerciseSet)mappedBack[1];
        timeSet.Name.Should().Be("Plank");
        timeSet.Duration.Should().Be(TimeSpan.FromSeconds(60));

        var bodyweightSet = (BodyweightExerciseSet)mappedBack[2];
        bodyweightSet.Name.Should().Be("Push-ups");
        bodyweightSet.Reps.Should().Be(15);

        var distanceSet = (DistanceExerciseSet)mappedBack[3];
        distanceSet.Name.Should().Be("5K Run");
        distanceSet.Distance.Should().Be(5.0f);
        distanceSet.DistanceUnit.Should().Be("km");

        var customSet = (CustomExerciseSet)mappedBack[4];
        customSet.Name.Should().Be("Rope Climbing");
        customSet.CustomValue.Should().Be("3");
        customSet.CustomUnit.Should().Be("ascents");
    }
}
