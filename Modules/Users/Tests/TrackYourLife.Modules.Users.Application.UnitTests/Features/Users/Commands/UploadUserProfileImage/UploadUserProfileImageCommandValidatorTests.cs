using FluentAssertions;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UploadUserProfileImage;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UploadUserProfileImage;

public sealed class UploadUserProfileImageCommandValidatorTests
{
    private readonly UploadUserProfileImageCommandValidator _validator;

    public UploadUserProfileImageCommandValidatorTests()
    {
        _validator = new UploadUserProfileImageCommandValidator();
    }

    [Fact]
    public void Validate_WhenFileIsEmpty_ReturnsFailure()
    {
        // Arrange
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(0);
        var command = new UploadUserProfileImageCommand(file);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result
            .Errors.Should()
            .Contain(x => x.PropertyName == "File" && x.ErrorMessage == "File cannot be empty");
    }

    [Fact]
    public void Validate_WhenFileSizeExceedsLimit_ReturnsFailure()
    {
        // Arrange
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(6 * 1024 * 1024); // 6MB
        file.FileName.Returns("test.jpg");
        var command = new UploadUserProfileImageCommand(file);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result
            .Errors.Should()
            .Contain(x =>
                x.PropertyName == "File"
                && x.ErrorMessage.Contains("File size must be less than 5MB")
            );
    }

    [Fact]
    public void Validate_WhenFileTypeIsNotAllowed_ReturnsFailure()
    {
        // Arrange
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(1024); // 1KB
        file.FileName.Returns("test.pdf");
        var command = new UploadUserProfileImageCommand(file);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result
            .Errors.Should()
            .Contain(x =>
                x.PropertyName == "File"
                && x.ErrorMessage.Contains("File must be one of the following types")
            );
    }

    [Fact]
    public void Validate_WhenFileIsValid_ReturnsSuccess()
    {
        // Arrange
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(1024); // 1KB
        file.FileName.Returns("test.jpg");
        var command = new UploadUserProfileImageCommand(file);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
