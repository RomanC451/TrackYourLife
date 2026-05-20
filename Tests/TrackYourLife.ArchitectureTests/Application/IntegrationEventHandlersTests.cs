using NetArchTest.Rules;
using TrackYourLife.SharedLib.Application.Abstraction.Messaging;

namespace TrackYourLife.ArchitectureTests.Application;

public class IntegrationEventHandlersTests : BaseArchitectureTest
{
    private static IEnumerable<Type> IntegrationEventHandlerTypes =>
        Types
            .InAssemblies(EventHandlerApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IIntegrationEventHandler<>))
            .And()
            .AreNotInterfaces()
            .GetTypes();

    [Fact]
    public void IntegrationEventHandlers_ShouldBeInternal() => ShouldBeInternal(IntegrationEventHandlerTypes);

    [Fact]
    public void IntegrationEventHandlers_ShouldBeSealed() => ShouldBeSealed(IntegrationEventHandlerTypes);

    [Fact]
    public void IntegrationEventHandlers_ShouldHaveIntegrationEventHandlerPostfix() =>
        ShouldHavePostfix(IntegrationEventHandlerTypes, "IntegrationEventHandler");
}
