using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests.Infrastructure.Data;

public class ConfigurationsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> ConfigurationsTypes =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .GetTypes();

    [Fact]
    public void Repositories_ShouldBeSealed() => ShouldBeSealed(ConfigurationsTypes);

    [Fact]
    public void Repositories_ShouldBeInternal() => ShouldBeInternal(ConfigurationsTypes);

    [Fact]
    public void Repositories_ShouldHaveRepositoryPostfix() =>
        ShouldHavePostfix(ConfigurationsTypes, "Configuration");
}
