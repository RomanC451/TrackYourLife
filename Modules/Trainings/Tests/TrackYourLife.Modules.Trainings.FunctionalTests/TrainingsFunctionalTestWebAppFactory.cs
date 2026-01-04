using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Trainings.FunctionalTests.Mocks;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Trainings.FunctionalTests;

public class TrainingsFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private TrainingsFunctionalTestCollection? _collection;

    public TrainingsFunctionalTestWebAppFactory()
        : base($"TrainingsDb-FunctionalTests") { }

    public override string? TestingSettingsFileName => null;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            // Replace the real Supabase storage with a mock for testing
            services.AddScoped<ISupaBaseStorage, MockSupaBaseStorage>();
        });
    }

    public void SetCollection(TrainingsFunctionalTestCollection collection)
    {
        _collection = collection;
    }

    public TrainingsFunctionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new TrainingsFunctionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }
}
