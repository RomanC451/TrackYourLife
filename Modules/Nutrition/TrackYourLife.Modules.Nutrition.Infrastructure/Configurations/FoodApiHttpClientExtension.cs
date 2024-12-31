using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Nutrition.Domain.Core;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Configurations;

/// <summary>
/// Provides extension methods to configure the HttpClient for the Food API.
/// </summary>
public static class FoodApiHttpClientExtension
{
    /// <summary>
    /// Configures the HttpClient for the Food API.
    /// </summary>
    /// <param name="httpClientBuilder">The IHttpClientBuilder instance.</param>
    /// <returns>The IHttpClientBuilder instance.</returns>
    /// <summary>
    public static IHttpClientBuilder ConfigureFoodApiHttpClient(
        this IHttpClientBuilder httpClientBuilder
    )
    {
        httpClientBuilder
            .ConfigureHttpClient(
                (serviceProvider, httpClient) =>
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                    httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    httpClient.DefaultRequestHeaders.Add(
                        "User-agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.104 Safari/537.36"
                    );
                }
            )
            .ConfigurePrimaryHttpMessageHandler(
                (serviceProvider) =>
                {


                    var cookieContainer =
                        serviceProvider.GetRequiredService<FoodApiCookieContainer>();

                    return new HttpClientHandler()
                    {
                        CookieContainer = cookieContainer,
                        AutomaticDecompression =
                            DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    };
                }
            )
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        return httpClientBuilder;
    }
}
