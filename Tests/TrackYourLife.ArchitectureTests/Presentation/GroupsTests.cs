using FastEndpoints;
using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests.Presentation
{
    public class GroupsTests : BaseArchitectureTest
    {
        private static IEnumerable<Type> GroupsTypes =>
            Types
                .InAssemblies(PresentationAssemblies.Assemblies)
                .That()
                .Inherit(typeof(Group))
                .GetTypes();

        [Fact]
        public void Groups_ShouldBeInternal() => ShouldBeInternal(GroupsTypes);

        [Fact]
        public void Groups_ShouldBeSealed() => ShouldBeSealed(GroupsTypes);
    }
}
