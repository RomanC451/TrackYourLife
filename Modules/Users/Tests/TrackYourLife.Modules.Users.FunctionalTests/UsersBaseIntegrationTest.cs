using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Users.FunctionalTests;

[Collection("Users Integration Tests")]
public class UsersBaseIntegrationTest(UsersFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection())
{
    protected override Task CleanupDatabaseAsync()
    {
        return Task.CompletedTask;
    }
}
