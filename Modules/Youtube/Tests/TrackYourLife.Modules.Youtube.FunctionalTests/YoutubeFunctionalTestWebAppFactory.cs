using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Youtube.FunctionalTests;

public class YoutubeFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private YoutubeFunctionalTestCollection? _collection;

    public YoutubeFunctionalTestWebAppFactory()
        : base("YoutubeDb-FunctionalTests") { }

    public override string? TestingSettingsFileName => "appsettings.Youtube.Testing.json";

    public void SetCollection(YoutubeFunctionalTestCollection collection)
    {
        _collection = collection;
    }

    public YoutubeFunctionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new YoutubeFunctionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }
}
