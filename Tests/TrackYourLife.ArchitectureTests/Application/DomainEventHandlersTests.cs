using MediatR;
using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests.Application;

public class DomainEventHandlersTests : BaseArchitectureTest
{
    private static IEnumerable<Type> DomainEventHandlerTypes =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(INotificationHandler<>))
            .GetTypes();

    [Fact]
    public void DomainEventHandlers_ShouldBeInternal() => ShouldBeInternal(DomainEventHandlerTypes);

    [Fact]
    public void DomainEventHandlers_ShouldBeSealed() => ShouldBeSealed(DomainEventHandlerTypes);

    [Fact]
    public void DomainEventHandlers_ShouldHaveDomainEventHandlerPostfix() =>
        ShouldHavePostfix(DomainEventHandlerTypes, "DomainEventHandler");
}
