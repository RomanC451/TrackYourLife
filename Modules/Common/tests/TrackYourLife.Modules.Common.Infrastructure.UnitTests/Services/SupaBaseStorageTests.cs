using Microsoft.AspNetCore.Http;
using Serilog;
using Supabase;
using Supabase.Storage;
using Supabase.Storage.Interfaces;
using TrackYourLife.Modules.Common.Application.Core.Abstraction;
using TrackYourLife.Modules.Common.Infrastructure.Services;
using TrackYourLife.SharedLib.Domain.Errors;
using Xunit;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Services;

public class SupaBaseStorageTests
{
    private readonly IStorageFileApi<FileObject> _storageBucket;
    private readonly ILogger _logger;
    private readonly SupaBaseStorage _sut;
    private readonly ISupabaseClient _supabaseClient;

    public SupaBaseStorageTests()
    {
        _storageBucket = Substitute.For<IStorageFileApi<FileObject>>();
        _logger = Substitute.For<ILogger>();
        _supabaseClient = Substitute.For<ISupabaseClient>();
        _supabaseClient.Storage.From(Arg.Any<string>()).Returns(_storageBucket);

        _sut = new SupaBaseStorage(_supabaseClient, _logger);
    }

    [Fact]
    public async Task UploadFileAsync_WithValidFile_ShouldUploadSuccessfully()
    {
        // Arrange
        const string bucketName = "test-bucket";
        const string fileName = "test.txt";
        const string contentType = "text/plain";
        var fileContent = "test content";
        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);

        var formFile = new FormFile(
            new MemoryStream(fileBytes),
            0,
            fileBytes.Length,
            fileName,
            fileName
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };

        _storageBucket.List().Returns(new List<FileObject>());

        // Act
        var result = await _sut.UploadFileAsync(bucketName, formFile, fileName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        await _storageBucket.Received(1).Upload(Arg.Any<byte[]>(), fileName);
        _logger
            .Received(1)
            .Information("File {FileName} uploaded to bucket {BucketName}", fileName, bucketName);
    }

    [Fact]
    public async Task UploadFileAsync_WithExistingFileAndReplace_ShouldUpdateFile()
    {
        // Arrange
        const string bucketName = "test-bucket";
        const string fileName = "test.txt";
        const string contentType = "text/plain";
        var fileContent = "test content";
        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);

        var formFile = new FormFile(
            new MemoryStream(fileBytes),
            0,
            fileBytes.Length,
            fileName,
            fileName
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };

        _storageBucket.List().Returns(new List<FileObject> { new() { Name = fileName } });

        // Act
        var result = await _sut.UploadFileAsync(bucketName, formFile, fileName, true);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        await _storageBucket
            .Received(1)
            .Update(Arg.Any<byte[]>(), fileName, Arg.Any<Supabase.Storage.FileOptions>());
        _logger
            .Received(1)
            .Information("File {FileName} updated in bucket {BucketName}", fileName, bucketName);
    }

    [Fact]
    public async Task UploadFileAsync_WithExistingFileAndNoReplace_ShouldReturnFailure()
    {
        // Arrange
        const string bucketName = "test-bucket";
        const string fileName = "test.txt";
        const string contentType = "text/plain";
        var fileContent = "test content";
        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);

        var formFile = new FormFile(
            new MemoryStream(fileBytes),
            0,
            fileBytes.Length,
            fileName,
            fileName
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };

        _storageBucket.List().Returns(new List<FileObject> { new() { Name = fileName } });

        // Act
        var result = await _sut.UploadFileAsync(bucketName, formFile, fileName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(InfrastructureErrors.SupaBaseClient.FileExists);
        await _storageBucket.DidNotReceive().Upload(Arg.Any<byte[]>(), Arg.Any<string>());
        await _storageBucket
            .DidNotReceive()
            .Update(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<Supabase.Storage.FileOptions>());
    }

    [Fact]
    public async Task GetAllFilesNamesFromBucketAsync_WithFiles_ShouldReturnFileNames()
    {
        // Arrange
        const string bucketName = "test-bucket";
        var files = new List<FileObject>
        {
            new() { Name = "file1.txt" },
            new() { Name = "file2.txt" },
        };

        _storageBucket.List().Returns(files);

        // Act
        var result = await _sut.GetAllFilesNamesFromBucketAsync(bucketName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo("file1.txt", "file2.txt");
    }

    [Fact]
    public async Task GetAllFilesNamesFromBucketAsync_WithEmptyBucketAndFailIfEmpty_ShouldReturnFailure()
    {
        // Arrange
        const string bucketName = "test-bucket";
        _storageBucket.List().Returns(new List<FileObject>());

        // Act
        var result = await _sut.GetAllFilesNamesFromBucketAsync(bucketName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(InfrastructureErrors.SupaBaseClient.NoFilesInBucket);
        _logger.Received(1).Warning("No files found in bucket {BucketName}", bucketName);
    }

    [Fact]
    public async Task GetAllFilesNamesFromBucketAsync_WithEmptyBucketAndNoFailIfEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        const string bucketName = "test-bucket";
        _storageBucket.List().Returns(new List<FileObject>());

        // Act
        var result = await _sut.GetAllFilesNamesFromBucketAsync(bucketName, false);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task DownloadFileAsync_WithValidFile_ShouldReturnFormFile()
    {
        // Arrange
        const string bucketName = "test-bucket";
        const string fileName = "test.txt";
        const string contentType = "text/plain";
        var fileContent = "test content";
        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);

        _storageBucket.Download(fileName, null).Returns(fileBytes);

        // Act
        var result = await _sut.DownloadFileAsync(bucketName, fileName, contentType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FileName.Should().Be(fileName);
        result.Value.ContentType.Should().Be(contentType);
        _logger
            .Received(1)
            .Information(
                "Downloading file {FileName} from bucket {BucketName}",
                fileName,
                bucketName
            );
    }

    [Fact]
    public async Task DownloadFileAsync_WithNonExistentFile_ShouldReturnFailure()
    {
        // Arrange
        const string bucketName = "test-bucket";
        const string fileName = "nonexistent.txt";
        const string contentType = "text/plain";

        _storageBucket.Download(fileName, null).Returns(Array.Empty<byte>());

        // Act
        var result = await _sut.DownloadFileAsync(bucketName, fileName, contentType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(InfrastructureErrors.SupaBaseClient.FileNotFound);
    }

    [Fact]
    public async Task DownloadFileAsync_WhenClientFails_ShouldReturnFailure()
    {
        // Arrange
        const string bucketName = "test-bucket";
        const string fileName = "test.txt";
        const string contentType = "text/plain";

        _storageBucket.Download(fileName, null).Returns<byte[]>(_ => throw new Exception());

        // Act
        var result = await _sut.DownloadFileAsync(bucketName, fileName, contentType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(InfrastructureErrors.SupaBaseClient.ClientNotWorking);
        _logger
            .Received(1)
            .Error(
                Arg.Any<Exception>(),
                "Failed to download file {FileName} from bucket {BucketName}",
                fileName,
                bucketName
            );
    }

    [Fact]
    public async Task DownloadFileAsync_WithNullFile_ShouldReturnFailure()
    {
        // Arrange
        const string bucketName = "test-bucket";
        const string fileName = "test.txt";
        const string contentType = "text/plain";

        _storageBucket.Download(fileName, null).Returns(Array.Empty<byte>());

        // Act
        var result = await _sut.DownloadFileAsync(bucketName, fileName, contentType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(InfrastructureErrors.SupaBaseClient.FileNotFound);
    }
}
