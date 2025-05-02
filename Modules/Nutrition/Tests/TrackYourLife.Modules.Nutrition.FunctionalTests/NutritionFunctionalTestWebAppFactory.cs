using Testcontainers.PostgreSql;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests;

public class NutritionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private NutritionFucntionalTestCollection? _collection;

    public NutritionFunctionalTestWebAppFactory()
        : base($"NutritionDb-FunctionalTests-{Guid.NewGuid()}") { }

    public void SetCollection(NutritionFucntionalTestCollection collection)
    {
        _collection = collection;
    }

    public NutritionFucntionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new NutritionFucntionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }
}
