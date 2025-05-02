using FluentAssertions;
using NetArchTest.Rules;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.ArchitectureTests.Infrastructure
{
    public class InfrastructureTests : BaseArchitectureTest
    {
        private static readonly string[] ExcludedClassNames =
        [
            "AssemblyReference",
            "ConfigureApp",
            "ConfigureServices",
            "ApiFood",
            "ApiServingSize",
            "AuthData",
            "FoodApiResponse",
            "ItemListElement",
        ];

        private static IEnumerable<Type> InfrastructureClasses =>
            Types
                .InAssemblies(InfrastructureAssemblies.Assemblies)
                .That()
                .AreClasses()
                .And()
                .DoNotImplementInterface(typeof(IOptions))
                .GetTypes()
                .Where(t =>
                    !ExcludedClassNames.Contains(t.Name)
                    && !t.Namespace?.Contains("Migrations") == true
                    && !t.Name?.Contains("DbContext") == true
                );

        [Fact]
        public void InfrastructureClasses_ShouldBeInternal() =>
            ShouldBeInternal(InfrastructureClasses);

        [Fact]
        public void InfrastructureClasses_ShouldNotDependOnApplicationLayer() =>
            Types
                .InAssemblies(InfrastructureAssemblies.Assemblies)
                .That()
                .AreClasses()
                .ShouldNot()
                .HaveDependencyOn("TrackYourLife.Application")
                .GetResult()
                .IsSuccessful.Should()
                .BeTrue();

        [Fact]
        public void InfrastructureClasses_ShouldNotDependOnDomainLayer() =>
            Types
                .InAssemblies(InfrastructureAssemblies.Assemblies)
                .That()
                .AreClasses()
                .ShouldNot()
                .HaveDependencyOn("TrackYourLife.Domain")
                .GetResult()
                .IsSuccessful.Should()
                .BeTrue();
    }
}
