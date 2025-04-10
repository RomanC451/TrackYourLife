using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests;

public class NutritionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private NutritionFucntionalTestCollection? _collection;

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
