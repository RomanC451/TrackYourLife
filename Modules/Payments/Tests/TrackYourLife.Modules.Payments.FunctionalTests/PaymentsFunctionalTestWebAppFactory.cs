using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Payments.FunctionalTests;

public class PaymentsFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private PaymentsFunctionalTestCollection? _collection;

    public PaymentsFunctionalTestWebAppFactory()
        : base("PaymentsDb-FunctionalTests") { }

    public override string? TestingSettingsFileName => null;

    public void SetCollection(PaymentsFunctionalTestCollection collection)
    {
        _collection = collection;
    }

    public PaymentsFunctionalTestCollection GetCollection()
    {
        if (_collection == null)
        {
            _collection = new PaymentsFunctionalTestCollection(this);
            _collection.InitializeAsync().GetAwaiter().GetResult();
        }
        return _collection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IStripeService));
            if (descriptor != null)
                services.Remove(descriptor);
            services.AddSingleton<IStripeService, MockStripeService>();
        });
    }
}
