using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Trainings;

public class TrainingIdTests
{
    [Fact]
    public void TrainingId_ShouldInheritFromStronglyTypedGuid()
    {
        // Arrange & Act
        var trainingIdType = typeof(TrainingId);
        var baseType = typeof(StronglyTypedGuid<TrainingId>);

        // Assert
        trainingIdType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void TrainingId_ShouldBeRecord()
    {
        // Arrange & Act
        var trainingIdType = typeof(TrainingId);

        // Assert
        trainingIdType.IsClass.Should().BeTrue();
        trainingIdType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithGuid_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var trainingId = new TrainingId(guid);

        // Assert
        trainingId.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateEmptyGuid()
    {
        // Act
        var trainingId = new TrainingId();

        // Assert
        trainingId.Value.Should().Be(Guid.Empty);
    }

    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789012", true)]
    [InlineData("invalid-guid", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void TryParse_WithValidInput_ShouldReturnCorrectResult(
        string? input,
        bool expectedSuccess
    )
    {
        // Act
        var result = TrainingId.TryParse(input, out var output);

        // Assert
        result.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            output.Should().NotBeNull();
            output!.Value.ToString().Should().Be(input);
        }
        else
        {
            output.Should().BeNull();
        }
    }

    [Fact]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var trainingId = new TrainingId(guid);

        // Act
        var result = trainingId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void TrainingId_ShouldBeEquatable()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var trainingId1 = new TrainingId(guid);
        var trainingId2 = new TrainingId(guid);
        var trainingId3 = new TrainingId(Guid.NewGuid());

        // Assert
        trainingId1.Should().Be(trainingId2);
        trainingId1.Should().NotBe(trainingId3);
        trainingId1.GetHashCode().Should().Be(trainingId2.GetHashCode());
    }
}

