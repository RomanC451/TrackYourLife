using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Services;

public sealed class FoodApiCookiesManager(
    IRequestClient<GetCookiesByDomainsRequest> getCookiesClient,
    IRequestClient<AddCookiesRequest> addCookiesClient,
    IRequestClient<AddCookiesFromFilesRequest> addCookiesFromFileClient,
    IOptions<FoodApiOptions> foodApiOptions
) : IFoodApiCookiesManager
{
    public async Task<List<Cookie>> GetCookiesFromDbAsync()
    {
        var response = await getCookiesClient.GetResponse<GetCookiesByDomainsResponse>(
            new GetCookiesByDomainsRequest(foodApiOptions.Value.CookieDomains)
        );

        return response.Message.Cookies;
    }

    public async Task<Result> AddCookiesToDbAsync(
        List<Cookie> cookies,
        CancellationToken cancellationToken
    )
    {
        var response = await addCookiesClient.GetResponse<AddCookiesResponse>(
            new AddCookiesRequest(cookies),
            cancellationToken
        );

        return response.Message.IsSuccess
            ? Result.Success()
            : Result.Failure(IntegrationErrors.MassTransit.FailedRequest);
    }

    public async Task<Result<List<Cookie>>> AddCookiesFromFilesToDbAsync(
        IFormFile CookieFile,
        CancellationToken cancellationToken
    )
    {
        var response = await addCookiesFromFileClient.GetResponse<
            AddCookiesFromFilesResponse,
            AddCookiesFromFilesErrorResponse
        >(new AddCookiesFromFilesRequest(await CookieFile.ToByteArrayAsync()), cancellationToken);

        if (response.Is(out Response<AddCookiesFromFilesErrorResponse>? errorResponse))
        {
            return Result.Failure<List<Cookie>>(errorResponse.Message.Error);
        }
        else if (response.Is(out Response<AddCookiesFromFilesResponse>? cookiesResponse))
        {
            return Result.Success(cookiesResponse.Message.Cookies);
        }
        return Result.Failure<List<Cookie>>(IntegrationErrors.MassTransit.FailedRequest);
    }
}
