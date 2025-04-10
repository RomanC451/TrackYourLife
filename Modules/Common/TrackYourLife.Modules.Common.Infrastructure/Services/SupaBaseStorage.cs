using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Supabase;
using TrackYourLife.Modules.Common.Application.Core.Abstraction;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Infrastructure.Services;

internal sealed class SupaBaseStorage(ISupabaseClient supabaseClient, ILogger logger)
    : ISupaBaseStorage
{
    public async Task<Result> UploadFileAsync(
        string bucketName,
        IFormFile file,
        string fileName,
        bool replaceIfExists = false
    )
    {
        logger.Information(
            "Uploading file {FileName} to bucket {BucketName}",
            fileName,
            bucketName
        );

        using var memoryStream = new MemoryStream();

        await file.CopyToAsync(memoryStream);

        var result = await GetAllFilesNamesFromBucketAsync(bucketName, false);

        if (result.IsFailure)
        {
            return Result.Failure(InfrastructureErrors.SupaBaseClient.ClientNotWorking);
        }

        var filesList = result.Value;

        if (filesList.Contains(fileName))
        {
            if (!replaceIfExists)
            {
                logger.Warning(
                    "File {FileName} already exists in bucket {BucketName}",
                    fileName,
                    bucketName
                );

                return Result.Failure(InfrastructureErrors.SupaBaseClient.FileExists);
            }

            await supabaseClient
                .Storage.From(bucketName)
                .Update(
                    memoryStream.ToArray(),
                    fileName,
                    new Supabase.Storage.FileOptions()
                    {
                        Upsert = true,
                        ContentType = file.ContentType,
                    }
                );

            logger.Information(
                "File {FileName} updated in bucket {BucketName}",
                fileName,
                bucketName
            );

            return Result.Success();
        }

        await supabaseClient.Storage.From(bucketName).Upload(memoryStream.ToArray(), fileName);

        logger.Information("File {FileName} uploaded to bucket {BucketName}", fileName, bucketName);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<string>>> GetAllFilesNamesFromBucketAsync(
        string bucketName,
        bool failIfEmpty = true
    )
    {
        var list = await supabaseClient.Storage.From(bucketName).List();

        if (list is null || list.IsNullOrEmpty())
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
