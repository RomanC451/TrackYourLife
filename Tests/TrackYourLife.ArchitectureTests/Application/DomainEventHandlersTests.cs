using MediatR;
using NetArchTest.Rules;
using TrackYourLife.SharedLib.Application.Abstraction.Messaging;

namespace TrackYourLife.ArchitectureTests.Application;

public class DomainEventHandlersTests : BaseArchitectureTest
{
    private static IEnumerable<Type> DomainEventHandlerTypes =>
        Types
            .InAssemblies(EventHandlerApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IDomainEventHandler<>))
            .And()
            .AreNotInterfaces()
            .GetTypes();

    [Fact]
    public void DomainEventHandlers_ShouldBeInternal() => ShouldBeInternal(DomainEventHandlerTypes);

    [Fact]
    public void DomainEventHandlers_ShouldBeSealed() => ShouldBeSealed(DomainEventHandlerTypes);

    [Fact]
    public void DomainEventHandlers_ShouldHaveDomainEventHandlerPostfix() =>
        ShouldHavePostfix(DomainEventHandlerTypes, "DomainEventHandler");
}
