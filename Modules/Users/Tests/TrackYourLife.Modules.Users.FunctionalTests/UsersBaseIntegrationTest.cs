using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Users.FunctionalTests;

[Collection("Users Integration Tests")]
public class UsersBaseIntegrationTest(UsersFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection())
{
    protected override async Task CleanupDatabaseAsync()
    {
        await CleanupDbSet(_usersWriteDbContext.OutboxMessages);
        await CleanupDbSet(_usersWriteDbContext.Goals);
    }

    protected async Task WaitForOutboxEventsToBeHandledAsync(
        CancellationToken cancellationToken = default
    )
    {
        await WaitForOutboxEventsToBeHandledAsync(
            _usersWriteDbContext.OutboxMessages,
            cancellationToken
        );
    }
}
