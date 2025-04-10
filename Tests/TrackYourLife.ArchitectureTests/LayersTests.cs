using FluentAssertions;
using NetArchTest.Rules;
using TrackYourLife.ArchitectureTests.Application;
using TrackYourLife.ArchitectureTests.Domain;
using TrackYourLife.ArchitectureTests.Infrastructure;
using TrackYourLife.ArchitectureTests.Presentation;

namespace TrackYourLife.ArchitectureTests;

public class LayersTests : BaseArchitectureTest
{
    [Fact]
    public void Domain_ShouldHaveNoDependenciesOnOtherLayers()
    {
        var result = Types
            .InAssemblies(DomainAssemblies.Assemblies)
            .Should()
            .NotHaveDependencyOnAny(
                [
                    .. PresentationAssemblies.Assemblies.Select(a => a.GetName().Name).ToArray(),
                    .. ApplicationAssemblies.Assemblies.Select(a => a.GetName().Name).ToArray(),
                    .. InfrastructureAssemblies.Assemblies.Select(a => a.GetName().Name).ToArray(),
                    SharedLib.Application.AssemblyReference.Assembly.GetName().Name,
                    SharedLib.Infrastructure.AssemblyReference.Assembly.GetName().Name,
                    SharedLib.Presentation.AssemblyReference.Assembly.GetName().Name,
                ]
            )
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes.Select(t => $"{t.FullName}").ToList();

            Assert.Fail(
                $"The following types have invalid dependencies:\n{string.Join("\n", failingTypes)}"
            );
        }

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldHaveNoDependenciesOnOtherLayers()
    {
        var result = Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .Should()
            .NotHaveDependencyOnAny(
                [
                    .. InfrastructureAssemblies.Assemblies.Select(a => a.GetName().Name).ToArray(),
                    .. PresentationAssemblies.Assemblies.Select(a => a.GetName().Name).ToArray(),
                    SharedLib.Infrastructure.AssemblyReference.Assembly.GetName().Name,
                    SharedLib.Presentation.AssemblyReference.Assembly.GetName().Name,
                ]
            )
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes.Select(t => $"{t.FullName}").ToList();

            Assert.Fail(
                $"The following types have invalid dependencies:\n{string.Join("\n", failingTypes)}"
            );
        }

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldHaveNoDependenciesOnOtherLayers()
    {
        var result = Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .Should()
            .NotHaveDependencyOnAny(
                [
                    .. PresentationAssemblies.Assemblies.Select(a => a.GetName().Name).ToArray(),
                    SharedLib.Presentation.AssemblyReference.Assembly.GetName().Name,
                ]
            )
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes.Select(t => $"{t.FullName}").ToList();

            Assert.Fail(
                $"The following types have invalid dependencies:\n{string.Join("\n", failingTypes)}"
            );
        }

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Presentation_ShouldHaveNoDependenciesOnOtherLayers()
    {
        var result = Types
            .InAssemblies(PresentationAssemblies.Assemblies)
            .Should()
            .NotHaveDependencyOnAny(
                [
                    .. InfrastructureAssemblies.Assemblies.Select(a => a.GetName().Name).ToArray(),
                    SharedLib.Infrastructure.AssemblyReference.Assembly.GetName().Name,
                ]
            )
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes.Select(t => $"{t.FullName}").ToList();

            Assert.Fail(
                $"The following types have invalid dependencies:\n{string.Join("\n", failingTypes)}"
            );
        }

        result.IsSuccessful.Should().BeTrue();
    }
}
