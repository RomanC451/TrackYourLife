using Microsoft.AspNetCore.Http;

namespace TrackYourLife.SharedLib.Application.Extensions;

public static class FileExtensions
{
    public static string GetExtension(this IFormFile file)
    {
        return Path.GetExtension(file.FileName).ToLowerInvariant();
    }

    public static async Task<byte[]> ToByteArrayAsync(this IFormFile formFile)
    {
        if (formFile == null)
        {
            throw new ArgumentNullException(nameof(formFile));
        }

        using var memoryStream = new MemoryStream();
        using var stream = formFile.OpenReadStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
