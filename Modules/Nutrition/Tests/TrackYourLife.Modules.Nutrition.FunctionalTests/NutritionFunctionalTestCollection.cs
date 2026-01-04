using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests;

[CollectionDefinition("Nutrition Integration Tests")]
public class NutritionFunctionalTestCollection(NutritionFunctionalTestWebAppFactory factory)
    : FunctionalTestCollection(factory),
        ICollectionFixture<NutritionFunctionalTestWebAppFactory>;
