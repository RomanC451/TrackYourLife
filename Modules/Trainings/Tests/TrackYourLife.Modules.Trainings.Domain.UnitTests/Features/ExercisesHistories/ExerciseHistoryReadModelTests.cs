using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Primitives;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.ExercisesHistories;

public class ExerciseHistoryReadModelTests
{
    [Fact]
    public void ExerciseHistoryReadModel_ShouldImplementIReadModel()
    {
        // Arrange & Act
        var readModelType = typeof(ExerciseHistoryReadModel);
        var interfaceType = typeof(IReadModel<ExerciseHistoryId>);

        // Assert
        readModelType.Should().BeAssignableTo(interfaceType);
    }

    [Fact]
    public void ExerciseHistoryReadModel_ShouldBeRecord()
    {
        // Arrange & Act
        var readModelType = typeof(ExerciseHistoryReadModel);

        // Assert
        readModelType.IsClass.Should().BeTrue();
        readModelType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = ExerciseHistoryId.Create(Guid.NewGuid());
        var ongoingTrainingId = OngoingTrainingId.Create(Guid.NewGuid());
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        var newExerciseSetsJson =
            "[{\"id\":\"12345678-1234-1234-1234-123456789012\",\"name\":\"Set 1\",\"count1\":12,\"unit1\":\"reps\",\"count2\":55.0,\"unit2\":\"kg\",\"orderIndex\":0}]";
        var oldExerciseSetsJson =
            "[{\"id\":\"12345678-1234-1234-1234-123456789012\",\"name\":\"Set 1\",\"count1\":10,\"unit1\":\"reps\",\"count2\":50.0,\"unit2\":\"kg\",\"orderIndex\":0}]";
        var areChangesApplied = false;
        var createdOnUtc = DateTime.UtcNow;
        var modifiedOnUtc = DateTime.UtcNow.AddMinutes(5);

        // Act
        var readModel = new ExerciseHistoryReadModel(
            id,
            ongoingTrainingId,
            exerciseId,
            newExerciseSetsJson,
            oldExerciseSetsJson,
            areChangesApplied,
            createdOnUtc,
            modifiedOnUtc
        );

        // Assert
        readModel.Id.Should().Be(id);
        readModel.OngoingTrainingId.Should().Be(ongoingTrainingId);
        readModel.ExerciseId.Should().Be(exerciseId);
        readModel.NewExerciseSetsJson.Should().Be(newExerciseSetsJson);
        readModel.OldExerciseSetsJson.Should().Be(oldExerciseSetsJson);
        readModel.AreChangesApplied.Should().Be(areChangesApplied);
        readModel.CreatedOnUtc.Should().Be(createdOnUtc);
        readModel.ModifiedOnUtc.Should().Be(modifiedOnUtc);
    }

    [Fact]
    public void Constructor_WithDefaultValues_ShouldUseDefaults()
    {
        // Arrange
        var id = ExerciseHistoryId.Create(Guid.NewGuid());
        var ongoingTrainingId = OngoingTrainingId.Create(Guid.NewGuid());
        var exerciseId = ExerciseId.Create(Guid.NewGuid());

        // Act
        var readModel = new ExerciseHistoryReadModel(id, ongoingTrainingId, exerciseId);

        // Assert
        readModel.Id.Should().Be(id);
        readModel.OngoingTrainingId.Should().Be(ongoingTrainingId);
        readModel.ExerciseId.Should().Be(exerciseId);
        readModel.NewExerciseSetsJson.Should().Be("[]");
        readModel.OldExerciseSetsJson.Should().Be("[]");
        readModel.AreChangesApplied.Should().BeFalse();
        readModel.CreatedOnUtc.Should().Be(default(DateTime));
        readModel.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void NewExerciseSets_ShouldDeserializeFromJson()
    {
        // Arrange
        var newExerciseSetsJson =
            @"[
            {
                ""Id"": ""12345678-1234-1234-1234-123456789012"",
                ""Name"": ""Set 1"",
                ""Count1"": 12,
                ""Unit1"": ""reps"",
                ""Count2"": 55.0,
                ""Unit2"": ""kg"",
                ""OrderIndex"": 0
            },
            {
                ""Id"": ""87654321-4321-4321-4321-210987654321"",
                ""Name"": ""Set 2"",
                ""Count1"": 7,
                ""Unit1"": ""reps"",
                ""Count2"": 57.5,
                ""Unit2"": ""kg"",
                ""OrderIndex"": 1
            }
        ]";

        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            newExerciseSetsJson
        );

        // Act
        var newExerciseSets = readModel.NewExerciseSets;

        // Assert
        newExerciseSets.Should().HaveCount(2);
        newExerciseSets[0].Id.Should().Be(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        newExerciseSets[0].Count1.Should().Be(12);
        newExerciseSets[0].Unit1.Should().Be("reps");
        newExerciseSets[0].Count2.Should().Be(55.0f);
        newExerciseSets[0].Unit2.Should().Be("kg");
        newExerciseSets[1].Id.Should().Be(Guid.Parse("87654321-4321-4321-4321-210987654321"));
        newExerciseSets[1].Count1.Should().Be(7);
        newExerciseSets[1].Unit1.Should().Be("reps");
        newExerciseSets[1].Count2.Should().Be(57.5f);
        newExerciseSets[1].Unit2.Should().Be("kg");
    }

    [Fact]
    public void ExerciseSetsBeforeChange_ShouldDeserializeFromJson()
    {
        // Arrange
        var oldExerciseSetsJson =
            @"[
            {
                ""Id"": ""12345678-1234-1234-1234-123456789012"",
                ""Name"": ""Set 1"",
                ""Count1"": 10,
                ""Unit1"": ""reps"",
                ""Count2"": 50.0,
                ""Unit2"": ""kg"",
                ""OrderIndex"": 0
            },
            {
                ""Id"": ""87654321-4321-4321-4321-210987654321"",
                ""Name"": ""Set 2"",
                ""Count1"": 8,
                ""Unit1"": ""reps"",
                ""Count2"": 60.0,
                ""Unit2"": ""kg"",
                ""OrderIndex"": 1
            }
        ]";

        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            "[]",
            oldExerciseSetsJson
        );

        // Act
        var oldExerciseSets = readModel.OldExerciseSets;

        // Assert
        oldExerciseSets.Should().HaveCount(2);
        oldExerciseSets[0].Name.Should().Be("Set 1");
        oldExerciseSets[0].Count1.Should().Be(10);
        oldExerciseSets[0].Unit1.Should().Be("reps");
        oldExerciseSets[0].Count2.Should().Be(50.0f);
        oldExerciseSets[0].Unit2.Should().Be("kg");
        oldExerciseSets[0].OrderIndex.Should().Be(0);
        oldExerciseSets[1].Name.Should().Be("Set 2");
        oldExerciseSets[1].Count1.Should().Be(8);
        oldExerciseSets[1].Unit1.Should().Be("reps");
        oldExerciseSets[1].Count2.Should().Be(60.0f);
        oldExerciseSets[1].Unit2.Should().Be("kg");
        oldExerciseSets[1].OrderIndex.Should().Be(1);
    }

    [Fact]
    public void NewExerciseSets_WithInvalidJson_ShouldThrowException()
    {
        // Arrange
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            "invalid json"
        );

        // Act & Assert
        // The actual implementation throws an exception for invalid JSON
        // This test should verify that the property getter throws JsonException
        var action = () => readModel.NewExerciseSets;
        action.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public void OldExerciseSets_WithInvalidJson_ShouldThrowException()
    {
        // Arrange
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid()),
            "[]",
            "invalid json"
        );

        // Act & Assert
        // The actual implementation throws an exception for invalid JSON
        // This test should verify that the property getter throws JsonException
        var action = () => readModel.OldExerciseSets;
        action.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public void NewExerciseSets_Init_ShouldSerializeToJson()
    {
        // Arrange
        var newExerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 55.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 7, "reps", 57.5f, "kg").Value,
        };

        // Act
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid())
        )
        {
            NewExerciseSets = newExerciseSets,
        };

        // Assert
        readModel.NewExerciseSetsJson.Should().NotBeNullOrEmpty();
        readModel.NewExerciseSets.Should().BeEquivalentTo(newExerciseSets);
    }

    [Fact]
    public void OldExerciseSets_Init_ShouldSerializeToJson()
    {
        // Arrange
        var oldExerciseSets = new List<ExerciseSet>
        {
            ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
            ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 8, "reps", 60.0f, "kg").Value,
        };

        // Act
        var readModel = new ExerciseHistoryReadModel(
            ExerciseHistoryId.Create(Guid.NewGuid()),
            OngoingTrainingId.Create(Guid.NewGuid()),
            ExerciseId.Create(Guid.NewGuid())
        )
        {
            OldExerciseSets = oldExerciseSets,
        };

        // Assert
        readModel.OldExerciseSetsJson.Should().NotBeNullOrEmpty();
        readModel.OldExerciseSets.Should().BeEquivalentTo(oldExerciseSets);
    }
}
