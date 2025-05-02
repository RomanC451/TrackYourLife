using Microsoft.AspNetCore.Http;

using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.Abstraction;

public interface ISupaBaseStorage
{
    Task<Result> UploadFileAsync(
        string bucketName,
        IFormFile file,
        string fileName,
        bool replaceIfExists = false
    );

    Task<Result<IEnumerable<string>>> GetAllFilesNamesFromBucketAsync(
        string bucketName,
        bool failIfEmpty = true
    );

    Task<Result<FormFile>> DownloadFileAsync(
        string bucketName,
        string fileName,
        string contentType
    );
}
