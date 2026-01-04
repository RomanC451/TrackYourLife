using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests;

public class NutritionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private NutritionFunctionalTestCollection? _collection;

    public NutritionFunctionalTestWebAppFactory()
        : base($"NutritionDb-FunctionalTests") { }

    public override string? TestingSettingsFileName => null;

    public void SetCollection(NutritionFunctionalTestCollection collection)
    {
        _collection = collection;
    }

    public NutritionFunctionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new NutritionFunctionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }
}
