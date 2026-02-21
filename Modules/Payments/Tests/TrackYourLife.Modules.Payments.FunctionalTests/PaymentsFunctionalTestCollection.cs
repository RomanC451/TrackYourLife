using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Payments.FunctionalTests;

[CollectionDefinition("Payments Integration Tests")]
public class PaymentsFunctionalTestCollection(PaymentsFunctionalTestWebAppFactory factory)
    : FunctionalTestCollection(factory),
        ICollectionFixture<PaymentsFunctionalTestWebAppFactory>;
