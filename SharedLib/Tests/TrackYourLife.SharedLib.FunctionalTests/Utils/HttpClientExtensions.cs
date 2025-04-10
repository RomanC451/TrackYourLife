using System.Net.Http.Json;

namespace TrackYourLife.SharedLib.FunctionalTests.Utils;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> DeleteAsJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        T value,
        CancellationToken cancellationToken = default
    )
    {
        var content = JsonContent.Create(value);
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = content };
        return await client.SendAsync(request, cancellationToken);
    }
}
