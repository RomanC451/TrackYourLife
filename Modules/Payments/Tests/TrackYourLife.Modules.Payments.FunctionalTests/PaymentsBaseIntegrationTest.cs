using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Payments.FunctionalTests;

[Collection("Payments Integration Tests")]
public class PaymentsBaseIntegrationTest(PaymentsFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection())
{
    protected override async Task CleanupDatabaseAsync()
    {
        await CleanupDbSet(_usersWriteDbContext.OutboxMessages);
        await CleanupDbSet(_usersWriteDbContext.Goals);
    }
}
