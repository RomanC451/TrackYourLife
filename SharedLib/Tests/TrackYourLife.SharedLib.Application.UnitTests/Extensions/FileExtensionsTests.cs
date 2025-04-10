using FluentAssertions;
using Microsoft.AspNetCore.Http;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.SharedLib.Application.UnitTests.Extensions;

public class FileExtensionsTests
{
    [Fact]
    public void GetExtension_WithValidFileName_ShouldReturnExtension()
    {
        // Arrange
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns("test.pdf");

        // Act
        var result = file.GetExtension();

        // Assert
        result.Should().Be(".pdf");
    }

    [Fact]
    public void GetExtension_WithFileNameWithoutExtension_ShouldReturnEmptyString()
    {
        // Arrange
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns("test");

        // Act
        var result = file.GetExtension();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetExtension_WithMultipleDots_ShouldReturnLastExtension()
    {
        // Arrange
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns("test.something.pdf");

        // Act
        var result = file.GetExtension();

        // Assert
        result.Should().Be(".pdf");
    }

    [Fact]
    public async Task ToByteArrayAsync_WithValidFile_ShouldReturnByteArray()
    {
        // Arrange
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var file = Substitute.For<IFormFile>();
        file.OpenReadStream().Returns(new MemoryStream(content));

        // Act
        var result = await file.ToByteArrayAsync();

        // Assert
        result.Should().BeEquivalentTo(content, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task ToByteArrayAsync_WithNullFile_ShouldThrowArgumentNullException()
    {
        // Arrange
        IFormFile? file = null;

        // Act
        var act = () => file!.ToByteArrayAsync();

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("formFile");
    }
}
