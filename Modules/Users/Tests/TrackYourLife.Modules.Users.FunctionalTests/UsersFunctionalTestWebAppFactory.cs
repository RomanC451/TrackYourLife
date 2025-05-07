using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Users.FunctionalTests;

public class UsersFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private UsersFucntionalTestCollection? _collection;

    public UsersFunctionalTestWebAppFactory()
        : base("UsersDb-FunctionalTests") { }

    public override string TestingSettingsFileName => "appsettings.Users.Testing.json";

    public void SetCollection(UsersFucntionalTestCollection collection)
    {
        _collection = collection;
    }

    public UsersFucntionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new UsersFucntionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }
}
