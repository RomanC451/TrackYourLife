using System.Net;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Application.Core.Abstractions.Services;

public interface ICookiesReader
{
    Task<Result<List<Cookie>>> GetCookiesAsync(
        byte[] cookieFileStream,
        CancellationToken cancellationToken
    );
}
