using System.Collections.Generic;
using System.Reflection;
using FastEndpoints;
using FluentAssertions;
using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests.Presentation;

public class EndpointsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> EndpointsTypes =>
        Types
            .InAssemblies(PresentationAssemblies.Assemblies)
            .That()
            .Inherit(typeof(Endpoint<,>))
            .GetTypes();

    [Fact]
    public void Endpoints_ShouldBeInternal() => ShouldBeInternal(EndpointsTypes);

    [Fact]
    public void Endpoints_ShouldBeSealed() => ShouldBeSealed(EndpointsTypes);
}
