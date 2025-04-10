using NetArchTest.Rules;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.ArchitectureTests.Infrastructure.Data;

public class QueriesTests : BaseArchitectureTest
{
    private static IEnumerable<Type> QueriesTypes =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .Inherit(typeof(GenericQuery<,>))
            .GetTypes();

    [Fact]
    public void Queries_ShouldBeSealed() => ShouldBeSealed(QueriesTypes);

    [Fact]
    public void Queries_ShouldBeInternal() => ShouldBeInternal(QueriesTypes);

    [Fact]
    public void Queries_ShouldHaveQueryPostfix() => ShouldHavePostfix(QueriesTypes, "Query");
}
