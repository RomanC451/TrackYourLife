using Microsoft.AspNetCore.Http;

namespace TrackYourLife.Common.Application.Core.Utils;

public static class FileExtension
{
    public static string GetExtension(this IFormFile file)
    {
        return Path.GetExtension(file.FileName).ToLowerInvariant();
    }
}
