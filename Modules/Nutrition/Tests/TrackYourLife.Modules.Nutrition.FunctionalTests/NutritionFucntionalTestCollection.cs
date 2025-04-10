using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests;

[CollectionDefinition("Nutrition Integration Tests")]
public class NutritionFucntionalTestCollection(NutritionFunctionalTestWebAppFactory factory)
    : FunctionalTestCollection(factory),
        ICollectionFixture<NutritionFunctionalTestWebAppFactory>;
