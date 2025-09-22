using Microsoft.AspNetCore.Http;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Mocks;

public class MockSupaBaseStorage : ISupaBaseStorage
{
    public Task<Result<string>> CreateSignedUrlAsync(string bucketName, string filePath)
    {
        // Return a mock URL for testing
        return Task.FromResult(Result.Success($"https://mock-storage.com/{bucketName}/{filePath}"));
    }

    public Task<Result<FormFile>> DownloadFileAsync(
        string bucketName,
        string fileName,
        string contentType
    )
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<string>>> GetAllFilesNamesFromBucketAsync(
        string bucketName,
        bool failIfEmpty = true
    )
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> RenameFileAsync(string currentPath, string newFileName)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> RenameFileFromSignedUrlAsync(string signedUrl, string newFileName)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> UploadFileAsync(
        string bucketName,
        IFormFile file,
        string path,
        bool replaceIfExists = false
    )
    {
        throw new NotImplementedException();
    }
}
