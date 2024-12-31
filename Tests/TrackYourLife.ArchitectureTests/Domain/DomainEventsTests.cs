using NetArchTest.Rules;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.ArchitectureTests.Domain;

public class DomainEventsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> DomainEventsTypes =>
        Types
            .InAssemblies(DomainAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .GetTypes();

    [Fact]
    public void DomainEvents_ShouldBeSealed() => ShouldBeSealed(DomainEventsTypes);

    [Fact]
    public void DomainEvents_ShouldBeDefinedAsRecords() =>
        ShouldBeDefinedAsRecords(DomainEventsTypes);

    [Fact]
    public void DomainEvents_ShouldHaveDomainEventPostfix() =>
        ShouldHavePostfix(DomainEventsTypes, "DomainEvent");
}
