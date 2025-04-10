using Microsoft.AspNetCore.Mvc.Testing;
using TrackYourLife.Modules.Users.FunctionalTests.TestServer;

namespace TrackYourLife.Modules.Users.FunctionalTests;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IntegrationTestWebAppFactory _factory;
    protected readonly HttpClient _client;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }
}
