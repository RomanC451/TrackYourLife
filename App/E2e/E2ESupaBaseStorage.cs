using Microsoft.AspNetCore.Http;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.App.E2e;

internal sealed class E2ESupaBaseStorage : ISupaBaseStorage
{
    public Task<Result<string>> CreateSignedUrlAsync(string bucketName, string filePath) =>
        Task.FromResult(
            Result.Success($"https://e2e-storage.local/{bucketName}/{filePath}")
        );

    public Task<Result<string>> UploadFileAsync(
        string bucketName,
        IFormFile file,
        string path,
        bool replaceIfExists = false
    ) => Task.FromResult(Result.Success($"https://e2e-storage.local/{bucketName}/{path}"));

    public Task<Result<IEnumerable<string>>> GetAllFilesNamesFromBucketAsync(
        string bucketName,
        bool failIfEmpty = true
    ) => Task.FromResult(Result.Success<IEnumerable<string>>([]));

    public Task<Result<string>> RenameFileAsync(string currentPath, string newFileName) =>
        Task.FromResult(Result.Success(newFileName));

    public Task<Result<string>> RenameFileFromSignedUrlAsync(string signedUrl, string newFileName) =>
        Task.FromResult(Result.Success(newFileName));

    public Task<Result<FormFile>> DownloadFileAsync(
        string bucketName,
        string fileName,
        string contentType
    ) =>
        Task.FromResult(
            Result.Success(
                new FormFile(Stream.Null, 0, 0, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType,
                }
            )
        );
}
