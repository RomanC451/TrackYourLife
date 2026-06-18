using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Reading.Infrastructure.Data;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Reading.FunctionalTests;

[Collection("Reading Integration Tests")]
public class ReadingBaseIntegrationTest(ReadingFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection())
{
    protected ReadingWriteDbContext ReadingWriteDbContext =>
        _scope.ServiceProvider.GetRequiredService<ReadingWriteDbContext>();

    protected override async Task CleanupDatabaseAsync()
    {
        await CleanupDbSet(ReadingWriteDbContext.ReadingSessions);
        await CleanupDbSet(ReadingWriteDbContext.Books);
        await CleanupDbSet(ReadingWriteDbContext.OutboxMessages);
        await CleanupDbSet(_usersWriteDbContext.Goals);
        await CleanupDbSet(_usersWriteDbContext.OutboxMessages);
    }
}
