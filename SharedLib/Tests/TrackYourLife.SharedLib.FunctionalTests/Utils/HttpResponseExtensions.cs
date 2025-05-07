using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            Converters = { new JsonStringEnumConverter() },
        };
        try
        {
            var responseContent = await JsonSerializer.DeserializeAsync<T>(
                await response.Content.ReadAsStreamAsync(),
                options
            );

            responseContent.Should().NotBeNull();
            return responseContent!;
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Failed to deserialize response content: {content}", ex);
        }
    }
}
