using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Users.FunctionalTests;

public class UsersFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private UsersFunctionalTestCollection? _collection;

    public UsersFunctionalTestWebAppFactory()
        : base("UsersDb-FunctionalTests") { }

    public override string? TestingSettingsFileName => null;

    public void SetCollection(UsersFunctionalTestCollection collection)
    {
        _collection = collection;
    }

    public UsersFunctionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new UsersFunctionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }
}
