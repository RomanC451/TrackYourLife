using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Reading.FunctionalTests;

public class ReadingFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private ReadingFunctionalTestCollection? _collection;

    public ReadingFunctionalTestWebAppFactory()
        : base("ReadingDb-FunctionalTests") { }

    public override string? TestingSettingsFileName => null;

    public void SetCollection(ReadingFunctionalTestCollection collection)
    {
        _collection = collection;
    }

    public ReadingFunctionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new ReadingFunctionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }
}
