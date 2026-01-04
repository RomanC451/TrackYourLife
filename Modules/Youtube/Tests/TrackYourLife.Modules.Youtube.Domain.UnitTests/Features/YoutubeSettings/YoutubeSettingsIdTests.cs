using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeSettings;

public class YoutubeSettingsIdTests
{
    [Fact]
    public void YoutubeSettingsId_ShouldInheritFromStronglyTypedGuid()
    {
        // Arrange & Act
        var idType = typeof(YoutubeSettingsId);
        var baseType = typeof(StronglyTypedGuid<YoutubeSettingsId>);

        // Assert
        idType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void YoutubeSettingsId_ShouldBeRecord()
    {
        // Arrange & Act
        var idType = typeof(YoutubeSettingsId);

        // Assert
        idType.IsClass.Should().BeTrue();
        idType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithGuid_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var id = new YoutubeSettingsId(guid);

        // Assert
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateEmptyGuid()
    {
        // Act
        var id = new YoutubeSettingsId();

        // Assert
        id.Value.Should().Be(Guid.Empty);
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
        var result = YoutubeSettingsId.TryParse(input, out var output);

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
        var id = new YoutubeSettingsId(guid);

        // Act
        var result = id.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void YoutubeSettingsId_ShouldBeEquatable()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = new YoutubeSettingsId(guid);
        var id2 = new YoutubeSettingsId(guid);
        var id3 = new YoutubeSettingsId(Guid.NewGuid());

        // Assert
        id1.Should().Be(id2);
        id1.Should().NotBe(id3);
        id1.GetHashCode().Should().Be(id2.GetHashCode());
    }
}
