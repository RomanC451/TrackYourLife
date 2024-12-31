using System.Net;
using Microsoft.AspNetCore.Http;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;

public interface IFoodApiCookiesManager
{
    Task<List<Cookie>> GetCookiesFromDbAsync();

    Task<Result> AddCookiesToDbAsync(List<Cookie> cookies, CancellationToken cancellationToken);

    Task<Result<List<Cookie>>> AddCookiesFromFilesToDbAsync(
        IFormFile CookieFile,
        CancellationToken cancellationToken
    );
}
