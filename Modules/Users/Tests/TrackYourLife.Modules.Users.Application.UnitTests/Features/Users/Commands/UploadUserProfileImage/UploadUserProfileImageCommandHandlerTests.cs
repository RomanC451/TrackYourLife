using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using TrackYourLife.Modules.Users.Application;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UploadUserProfileImage;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Commands.UploadUserProfileImage;

public sealed class UploadUserProfileImageCommandHandlerTests
{
    private readonly ISupaBaseStorage _supabaseStorage;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UploadUserProfileImageCommandHandler _handler;

    public UploadUserProfileImageCommandHandlerTests()
    {
        _supabaseStorage = Substitute.For<ISupaBaseStorage>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UploadUserProfileImageCommandHandler(
            _supabaseStorage,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenFileIsValid_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns("test.jpg");
        file.Length.Returns(1024); // 1KB
        var command = new UploadUserProfileImageCommand(file);
        var expectedFileName = $"user-{userId.Value}.jpg";

        _userIdentifierProvider.UserId.Returns(userId);
        _supabaseStorage
            .UploadFileAsync(
                SupaBaseStorageBuckets.UsersProfilesImages,
                file,
                expectedFileName,
                false
            )
            .Returns(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _supabaseStorage
            .Received(1)
            .UploadFileAsync(
                SupaBaseStorageBuckets.UsersProfilesImages,
                file,
                expectedFileName,
                false
            );
    }

    [Fact]
    public async Task Handle_WhenStorageUploadFails_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns("test.jpg");
        file.Length.Returns(1024); // 1KB
        var command = new UploadUserProfileImageCommand(file);
        var expectedFileName = $"user-{userId.Value}.jpg";
        var expectedError = Result.Failure(InfrastructureErrors.SupaBaseClient.ClientNotWorking);

        _userIdentifierProvider.UserId.Returns(userId);
        _supabaseStorage
            .UploadFileAsync(
                SupaBaseStorageBuckets.UsersProfilesImages,
                file,
                expectedFileName,
                false
            )
            .Returns(expectedError);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(expectedError.Error);
        await _supabaseStorage
            .Received(1)
            .UploadFileAsync(
                SupaBaseStorageBuckets.UsersProfilesImages,
                file,
                expectedFileName,
                false
            );
    }
}
