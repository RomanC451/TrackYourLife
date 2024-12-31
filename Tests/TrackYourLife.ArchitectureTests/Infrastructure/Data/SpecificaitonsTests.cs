using NetArchTest.Rules;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.ArchitectureTests.Infrastructure.Data;

public class SpecificaitonsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> SpecificationsTypes =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .Inherit(typeof(Specification<,>))
            .GetTypes();

    [Fact]
    public void Repositories_ShouldBeSealed() => ShouldBeSealed(SpecificationsTypes);

    [Fact]
    public void Repositories_ShouldBeInternal() => ShouldBeInternal(SpecificationsTypes);

    [Fact]
    public void Repositories_ShouldHaveRepositoryPostfix() =>
        ShouldHavePostfix(SpecificationsTypes, "Specification");
}
