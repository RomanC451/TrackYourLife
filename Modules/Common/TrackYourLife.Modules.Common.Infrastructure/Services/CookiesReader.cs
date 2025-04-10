using System.Net;
using TrackYourLife.Modules.Common.Application.Core.Abstraction.Services;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Infrastructure.Services;

internal sealed class CookiesReader() : ICookiesReader
{
    public async Task<Result<List<Cookie>>> GetCookiesAsync(
        byte[] cookieFileStream,
        CancellationToken cancellationToken
    )
    {
        List<Cookie> cookies = new List<Cookie>();

        using (MemoryStream memoryStream = new MemoryStream(cookieFileStream))
        using (StreamReader reader = new StreamReader(memoryStream))
        {
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                if (line == null || line.StartsWith('#') || string.IsNullOrWhiteSpace(line))
                {
                    continue; // Skip comments and empty lines
                }

                var parts = line.Split('\t');
                if (parts.Length == 7)
                {
                    string domain = parts[0];
                    string path = parts[2];
                    bool secure = parts[3].ToLower() == "true";
                    DateTime expiry =
                        parts[4] == "0"
                            ? DateTime.MinValue
                            : DateTimeOffset.FromUnixTimeSeconds(long.Parse(parts[4])).DateTime;
                    string name = parts[5];
                    string value = parts[6];

                    cookies.Add(
                        new Cookie(name, value, path, domain) { Secure = secure, Expires = expiry }
                    );
                }
            }
        }

        return cookies;
    }
}
