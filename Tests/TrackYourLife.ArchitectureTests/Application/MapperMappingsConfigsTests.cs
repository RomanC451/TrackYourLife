using Mapster;
using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests.Application;

public class MapperMappingsConfigsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> MapperMappingsConfigsTypes =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IRegister))
            .GetTypes();

    [Fact]
    public void MapperMappingsConfigs_ShouldBeInternal() =>
        ShouldBeInternal(MapperMappingsConfigsTypes);

    [Fact]
    public void MapperMappingsConfigs_ShouldBeSealed() =>
        ShouldBeSealed(MapperMappingsConfigsTypes);

    [Fact]
    public void MapperMappingsConfigs_ShouldHaveMapperMappingsConfigsPostfix() =>
        ShouldHavePostfix(MapperMappingsConfigsTypes, "MappingsConfig");
}
