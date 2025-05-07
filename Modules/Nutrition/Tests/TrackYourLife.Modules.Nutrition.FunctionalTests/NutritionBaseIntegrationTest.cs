using TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;
using TrackYourLife.SharedLib.FunctionalTests;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests;

[Collection("Nutrition Integration Tests")]
public class NutritionBaseIntegrationTest(NutritionFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection())
{
    protected override async Task CleanupDatabaseAsync()
    {
        await CleanupDbSet(_nutritionWriteDbContext.Recipes);
        await CleanupDbSet(_nutritionWriteDbContext.Foods);
        await CleanupDbSet(_nutritionWriteDbContext.ServingSizes);
        await CleanupDbSet(_nutritionWriteDbContext.FoodDiaries);
        await CleanupDbSet(_nutritionWriteDbContext.RecipeDiaries);
        await CleanupDbSet(_nutritionWriteDbContext.DailyNutritionOverviews);
        await CleanupDbSet(_nutritionWriteDbContext.FoodHistories);
        await CleanupDbSet(_nutritionWriteDbContext.SearchedFoods);
        await CleanupDbSet(_nutritionWriteDbContext.OutboxMessages);
    }

    protected async Task WaitForOutboxEventsToBeHandledAsync(
        CancellationToken cancellationToken = default
    )
    {
        await WaitForOutboxEventsToBeHandledAsync(
            _nutritionWriteDbContext.OutboxMessages,
            cancellationToken
        );
    }

    protected void SetupFoodApiMock(string searchQuery, FoodApiResponse response)
    {
        factory
            .WireMockServer.Given(Request.Create().WithPath("/user/auth_token?refresh=true"))
            .RespondWith(
                Response
                    .Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(
                        new AuthData()
                        {
                            AccessToken = "test",
                            ExpiresIn = 3600,
                            RefreshToken = "test",
                            TokenType = "Bearer",
                            UserId = "test",
                        }
                    )
            );

        factory
            .WireMockServer.Given(
                Request
                    .Create()
                    .WithPath("/v2/nutrition")
                    .WithParam("q", searchQuery)
                    .WithParam("max_items", "20")
            )
            .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(response));
    }
}
