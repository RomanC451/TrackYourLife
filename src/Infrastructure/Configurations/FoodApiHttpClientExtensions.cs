using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrackYourLifeDotnet.Infrastructure.Options;
using TrackYourLifeDotnet.Infrastructure.Utils;

namespace TrackYourLifeDotnet.Infrastructure.Configurations;

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
                    CookieContainer cookiesContainer = ChromeCookiesReader.GetCookies(
                        foodApiOptions.CookieDoamins
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
