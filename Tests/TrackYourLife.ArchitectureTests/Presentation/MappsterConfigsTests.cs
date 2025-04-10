using Mapster;
using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests.Presentation
{
    public class MappsterConfigsTests : BaseArchitectureTest
    {
        private static IEnumerable<Type> MappsterConfigsTypes =>
            Types
                .InAssemblies(PresentationAssemblies.Assemblies)
                .That()
                .ImplementInterface(typeof(IRegister))
                .GetTypes();

        [Fact]
        public void MappsterConfigs_ShouldBeInternal() => ShouldBeInternal(MappsterConfigsTypes);

        [Fact]
        public void MappsterConfigs_ShouldBeSealed() => ShouldBeSealed(MappsterConfigsTypes);

        [Fact]
        public void MappsterConfigs_ShouldHavePostfix() =>
            ShouldHavePostfix(MappsterConfigsTypes, "MappingsConfig");
    }
}
