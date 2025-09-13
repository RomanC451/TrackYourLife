using Microsoft.AspNetCore.Http;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.Abstraction;

public interface ISupaBaseStorage
{
    Task<Result<string>> UploadFileAsync(
        string bucketName,
        IFormFile file,
        string path,
        bool replaceIfExists = false
    );

    Task<Result<IEnumerable<string>>> GetAllFilesNamesFromBucketAsync(
        string bucketName,
        bool failIfEmpty = true
    );

    Task<Result<string>> CreateSignedUrlAsync(string bucketName, string filePath);

    Task<Result<string>> RenameFileAsync(string currentPath, string newFileName);

    Task<Result<string>> RenameFileFromSignedUrlAsync(string signedUrl, string newFileName);

    Task<Result<FormFile>> DownloadFileAsync(
        string bucketName,
        string fileName,
        string contentType
    );
}
