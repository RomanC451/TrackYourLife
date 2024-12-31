using NetArchTest.Rules;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.ArchitectureTests.Infrastructure.Data;

public class RepositoriesTests : BaseArchitectureTest
{
    private static IEnumerable<Type> RepositoriesTypes =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .Inherit(typeof(GenericRepository<,>))
            .GetTypes();

    [Fact]
    public void Repositories_ShouldBeSealed() => ShouldBeSealed(RepositoriesTypes);

    [Fact]
    public void Repositories_shouldBeInternal() => ShouldBeInternal(RepositoriesTypes);

    [Fact]
    public void Repositories_ShouldHaveRepositoryPostfix() =>
        ShouldHavePostfix(RepositoriesTypes, "Repository");
}
