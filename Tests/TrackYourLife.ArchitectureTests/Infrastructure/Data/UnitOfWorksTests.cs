using NetArchTest.Rules;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.ArchitectureTests.Infrastructure.Data;

public class UnitOfWorksTests : BaseArchitectureTest
{
    private static IEnumerable<Type> UnitOfWorksTypes =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .Inherit(typeof(UnitOfWork<>))
            .GetTypes();

    [Fact]
    public void Repositories_ShouldBeSealed() => ShouldBeSealed(UnitOfWorksTypes);

    [Fact]
    public void Repositories_ShouldBeInternal() => ShouldBeInternal(UnitOfWorksTypes);

    [Fact]
    public void Repositories_ShouldHaveRepositoryPostfix() =>
        ShouldHavePostfix(UnitOfWorksTypes, "UnitOfWork");
}
