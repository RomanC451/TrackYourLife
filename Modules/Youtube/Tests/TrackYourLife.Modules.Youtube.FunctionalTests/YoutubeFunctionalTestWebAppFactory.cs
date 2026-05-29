using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Youtube.FunctionalTests.Mocks;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Youtube.FunctionalTests;

public class YoutubeFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private YoutubeFunctionalTestCollection? _collection;

    public YoutubeFunctionalTestWebAppFactory()
        : base("YoutubeDb-FunctionalTests") { }

    public override string? TestingSettingsFileName => "appsettings.Youtube.Testing.json";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddScoped<IEmailService, SuccessfulEmailService>();
        });
    }

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
