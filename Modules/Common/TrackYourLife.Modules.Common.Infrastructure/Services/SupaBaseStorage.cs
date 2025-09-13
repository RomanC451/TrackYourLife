using Microsoft.AspNetCore.Http;
using Serilog;
using Supabase.Storage.Exceptions;
using TrackYourLife.Modules.Common.Application.Core.Abstraction;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Infrastructure.Services;

internal sealed class SupaBaseStorage(ISupabaseClient supabaseClient, ILogger logger)
    : ISupaBaseStorage
{
    public async Task<Result<string>> UploadFileAsync(
        string bucketName,
        IFormFile file,
        string path,
        bool replaceIfExists = false
    )
    {
        logger.Information("Uploading file {FileName} to bucket {BucketName}", path, bucketName);

        using var memoryStream = new MemoryStream();

        await file.CopyToAsync(memoryStream);

        var result = await GetAllFilesNamesFromBucketAsync(bucketName, false);

        if (result.IsFailure)
        {
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.ClientNotWorking);
        }

        var filesList = result.Value;

        if (filesList.Contains(path))
        {
            if (!replaceIfExists)
            {
                logger.Warning(
                    "File {FileName} already exists in bucket {BucketName}",
                    path,
                    bucketName
                );

                return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.FileExists);
            }

            var updatedUrl = await supabaseClient
                .Storage.From(bucketName)
                .Update(
                    memoryStream.ToArray(),
                    path,
                    new Supabase.Storage.FileOptions()
                    {
                        Upsert = true,
                        ContentType = file.ContentType,
                    }
                );

            logger.Information("File {FileName} updated in bucket {BucketName}", path, bucketName);

            return Result.Success(updatedUrl);
        }

        var url = await supabaseClient
            .Storage.From(bucketName)
            .Upload(memoryStream.ToArray(), path);

        logger.Information("File {FileName} uploaded to bucket {BucketName}", path, bucketName);

        return Result.Success(url);
    }

    public async Task<Result<IEnumerable<string>>> GetAllFilesNamesFromBucketAsync(
        string bucketName,
        bool failIfEmpty = true
    )
    {
        var list = await supabaseClient.Storage.From(bucketName).List();

        if (list is null || list.Count == 0)
        {
            if (failIfEmpty)
            {
                logger.Warning("No files found in bucket {BucketName}", bucketName);

                return Result.Failure<IEnumerable<string>>(
                    InfrastructureErrors.SupaBaseClient.NoFilesInBucket
                );
            }

            return Result.Success(Enumerable.Empty<string>());
        }

        return Result.Success(list.Select(file => file.Name ?? ""));
    }

    public async Task<Result<string>> CreateSignedUrlAsync(string bucketName, string filePath)
    {
        string url;

        try
        {
            url = await supabaseClient.Storage.From(bucketName).CreateSignedUrl(filePath, 10 * 60);
        }
        catch (SupabaseStorageException ex)
        {
            logger.Error(
                ex,
                "Failed to create signed url for file {FileName} in bucket {BucketName}",
                filePath,
                bucketName
            );
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.ClientNotWorking);
        }

        if (url is null)
        {
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.FileNotFound);
        }

        return Result.Success(url);
    }

    private static Result<string> GetPathFromSignedUrl(string signedUrl)
    {
        if (string.IsNullOrWhiteSpace(signedUrl))
        {
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.InvalidSignedUrl);
        }

        if (!Uri.TryCreate(signedUrl, UriKind.Absolute, out var uri))
        {
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.InvalidSignedUrl);
        }

        if (uri.Segments.Length < 2)
        {
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.InvalidSignedUrl);
        }
        string path;
        try
        {
            path = signedUrl.Split('?')[0].Split("sign/")[1];
        }
        catch (IndexOutOfRangeException)
        {
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.InvalidSignedUrl);
        }

        return Result.Success(path);
    }

    public async Task<Result<string>> RenameFileAsync(string currentPath, string newFileName)
    {
        var bucketName = currentPath.Split('/')[0];
        var fileName = currentPath.Split('/', 2)[1];

        var result = await supabaseClient.Storage.From(bucketName).Move(fileName, newFileName);

        if (!result)
        {
            return Result.Failure<string>(InfrastructureErrors.SupaBaseClient.FileNotFound);
        }

        return Result.Success(newFileName);
    }

    public async Task<Result<string>> RenameFileFromSignedUrlAsync(
        string signedUrl,
        string newFileName
    )
    {
        var pathResult = GetPathFromSignedUrl(signedUrl);

        if (pathResult.IsFailure)
        {
            return Result.Failure<string>(pathResult.Error);
        }

        return await RenameFileAsync(pathResult.Value, newFileName);
    }

    public async Task<Result<FormFile>> DownloadFileAsync(
        string bucketName,
        string fileName,
        string contentType
    )
    {
        logger.Information(
            "Downloading file {FileName} from bucket {BucketName}",
            fileName,
            bucketName
        );

        try
        {
            var fileBytes = await supabaseClient.Storage.From(bucketName).Download(fileName, null);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return Result.Failure<FormFile>(InfrastructureErrors.SupaBaseClient.FileNotFound);
            }

            var formFile = ConvertByteArrayToFormFile(fileBytes, fileName, contentType);
            return Result.Success(formFile);
        }
        catch (Exception ex)
        {
            logger.Error(
                ex,
                "Failed to download file {FileName} from bucket {BucketName}",
                fileName,
                bucketName
            );
            return Result.Failure<FormFile>(InfrastructureErrors.SupaBaseClient.ClientNotWorking);
        }
    }

    private static FormFile ConvertByteArrayToFormFile(
        byte[] fileBytes,
        string fileName,
        string contentType
    )
    {
        var stream = new MemoryStream(fileBytes);
        var formFile = new FormFile(stream, 0, fileBytes.Length, fileName, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };

        return formFile;
    }
}
