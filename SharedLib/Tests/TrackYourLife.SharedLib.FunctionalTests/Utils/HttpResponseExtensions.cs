using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

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

        var responseContent = await JsonSerializer.DeserializeAsync<T>(
            await response.Content.ReadAsStreamAsync(),
            options
        );
        responseContent.Should().NotBeNull();
        return responseContent!;
    }
}
