using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests;

public class ModulesTests : BaseArchitectureTest
{
    [Fact]
    public void Modules_ShouldHaveNoDependenciesOnOtherModules()
    {
        // For each module, check it doesn't depend on other modules
        foreach (var currentModuleAssemblies in ModulesAssemblies)
        {
            // Get all other module assemblies
            var otherModuleAssemblies = ModulesAssemblies
                .Where(m => m != currentModuleAssemblies)
                .SelectMany(m => m)
                .Select(a => a.GetName().Name)
                .ToArray();

            var result = Types
                .InAssemblies(currentModuleAssemblies)
                .Should()
                .NotHaveDependencyOnAny(otherModuleAssemblies)
                .GetResult();

            if (!result.IsSuccessful)
            {
                var failingTypes = result.FailingTypes.Select(t => $"{t.FullName}").ToList();
                Assert.Fail(
                    $"The following types in module {currentModuleAssemblies[0].GetName().Name} have invalid dependencies:\n{string.Join("\n", failingTypes)}"
                );
            }

            Assert.True(result.IsSuccessful);
        }
    }
}
