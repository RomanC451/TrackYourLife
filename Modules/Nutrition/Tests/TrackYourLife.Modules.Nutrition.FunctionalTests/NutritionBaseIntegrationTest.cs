using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests;

[Collection("Nutrition Integration Tests")]
public class NutritionBaseIntegrationTest(NutritionFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection());
