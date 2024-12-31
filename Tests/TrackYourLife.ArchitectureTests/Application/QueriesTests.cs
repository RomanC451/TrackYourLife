using NetArchTest.Rules;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.ArchitectureTests.Application;

public class QueriesTests : BaseArchitectureTest
{
    private static IEnumerable<Type> QueryTypes =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .GetTypes();

    [Fact]
    public void Queries_ShouldBeSealed() => ShouldBeSealed(QueryTypes);

    [Fact]
    public void Queries_ShouldHaveQueryPostfix() => ShouldHavePostfix(QueryTypes, "Query");

    [Fact]
    public void Queries_ShouldBeDefinedAsRecords() => ShouldBeDefinedAsRecords(QueryTypes);
}
