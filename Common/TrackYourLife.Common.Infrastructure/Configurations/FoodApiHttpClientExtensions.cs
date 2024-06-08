using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using TrackYourLife.Common.Infrastructure.Options;
using TrackYourLife.Common.Infrastructure.Utils;

namespace TrackYourLife.Common.Infrastructure.Configurations;

public static class FoodApiHttpClientExtensions
{
    public static IHttpClientBuilder ConfigureFoodApiHttpClient(
        this IHttpClientBuilder httpClientBuilder
    )
    {
        httpClientBuilder
            .ConfigureHttpClient(
                (serviceProvider, httpClient) =>
                {
                    var foodApiOptions = serviceProvider
                        .GetRequiredService<IOptions<FoodApiOptions>>()
                        .Value;
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
                    var foodApiOptions = serviceProvider
                        .GetRequiredService<IOptions<FoodApiOptions>>()
                        .Value;

                    var logger = serviceProvider.GetRequiredService<ILogger>();

                    CookieContainer cookiesContainer = new ChromeCookiesReader(logger).GetCookies(
                        foodApiOptions.CookieDomains
                    );
                    return new HttpClientHandler()
                    {
                        CookieContainer = cookiesContainer,
                        AutomaticDecompression =
                            DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    };
                }
            )
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        return httpClientBuilder;
    }
}
