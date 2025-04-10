using System.Net;
using System.Net.Http.Json;

namespace TrackYourLife.SharedLib.FunctionalTests.Utils;

public static class HttpResponseExtensions
{
    public static async Task<HttpResponseMessage> ShouldHaveStatusCode(
        this HttpResponseMessage response,
        HttpStatusCode expectedStatusCode
    )
    {
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(expectedStatusCode, $"Response content: {content}");
        return response;
    }

    public static async Task<T> ShouldHaveStatusCodeAndContent<T>(
        this HttpResponseMessage response,
        HttpStatusCode expectedStatusCode
    )
    {
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(expectedStatusCode, $"Response content: {content}");
        var responseContent = await response.Content.ReadFromJsonAsync<T>();
        responseContent.Should().NotBeNull();
        return responseContent!;
    }
}
