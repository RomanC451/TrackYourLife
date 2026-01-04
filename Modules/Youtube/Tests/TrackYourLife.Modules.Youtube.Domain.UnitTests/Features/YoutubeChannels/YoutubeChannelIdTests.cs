using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeChannels;

public class YoutubeChannelIdTests
{
    [Fact]
    public void YoutubeChannelId_ShouldInheritFromStronglyTypedGuid()
    {
        // Arrange & Act
        var idType = typeof(YoutubeChannelId);
        var baseType = typeof(StronglyTypedGuid<YoutubeChannelId>);

        // Assert
        idType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void YoutubeChannelId_ShouldBeRecord()
    {
        // Arrange & Act
        var idType = typeof(YoutubeChannelId);

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
        var id = new YoutubeChannelId(guid);

        // Assert
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Default_ShouldCreateEmptyGuid()
    {
        // Act
        var id = new YoutubeChannelId();

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
        var result = YoutubeChannelId.TryParse(input, out var output);

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
        var id = new YoutubeChannelId(guid);

        // Act
        var result = id.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void YoutubeChannelId_ShouldBeEquatable()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id1 = new YoutubeChannelId(guid);
        var id2 = new YoutubeChannelId(guid);
        var id3 = new YoutubeChannelId(Guid.NewGuid());

        // Assert
        id1.Should().Be(id2);
        id1.Should().NotBe(id3);
        id1.GetHashCode().Should().Be(id2.GetHashCode());
    }
}
