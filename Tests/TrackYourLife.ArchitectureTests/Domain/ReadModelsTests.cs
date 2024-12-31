using NetArchTest.Rules;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.ArchitectureTests.Domain;

public class ReadModelsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> ReadModelTypes =>
        Types
            .InAssemblies(DomainAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IReadModel<>))
            .GetTypes();

    [Fact]
    public void ReadModels_ShouldBeSealed() => ShouldBeSealed(ReadModelTypes);

    [Fact]
    public void ReadModels_ShouldBeDefinedAsRecords() => ShouldBeDefinedAsRecords(ReadModelTypes);

    [Fact]
    public void ReadModels_ShouldHaveReadModelPostfix() =>
        ShouldHavePostfix(ReadModelTypes, "ReadModel");
}
