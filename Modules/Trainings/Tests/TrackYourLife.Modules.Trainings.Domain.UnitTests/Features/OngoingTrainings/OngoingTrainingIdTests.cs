using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.OngoingTrainings;

public class OngoingTrainingIdTests
{
    [Fact]
    public void OngoingTrainingId_ShouldInheritFromStronglyTypedGuid()
    {
        // Arrange & Act
        var ongoingTrainingIdType = typeof(OngoingTrainingId);
        var baseType = typeof(StronglyTypedGuid<OngoingTrainingId>);

        // Assert
        ongoingTrainingIdType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void OngoingTrainingId_ShouldBeRecord()
    {
        // Arrange & Act
        var ongoingTrainingIdType = typeof(OngoingTrainingId);

        // Assert
        ongoingTrainingIdType.IsClass.Should().BeTrue();
        ongoingTrainingIdType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithGuid_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var ongoingTrainingId = new OngoingTrainingId(guid);

        // Assert
        ongoingTrainingId.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateEmptyGuid()
    {
        // Act
        var ongoingTrainingId = new OngoingTrainingId();

        // Assert
        ongoingTrainingId.Value.Should().Be(Guid.Empty);
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
        var result = OngoingTrainingId.TryParse(input, out var output);

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
        var ongoingTrainingId = new OngoingTrainingId(guid);

        // Act
        var result = ongoingTrainingId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void OngoingTrainingId_ShouldBeEquatable()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var ongoingTrainingId1 = new OngoingTrainingId(guid);
        var ongoingTrainingId2 = new OngoingTrainingId(guid);
        var ongoingTrainingId3 = new OngoingTrainingId(Guid.NewGuid());

        // Assert
        ongoingTrainingId1.Should().Be(ongoingTrainingId2);
        ongoingTrainingId1.Should().NotBe(ongoingTrainingId3);
        ongoingTrainingId1.GetHashCode().Should().Be(ongoingTrainingId2.GetHashCode());
    }
}

