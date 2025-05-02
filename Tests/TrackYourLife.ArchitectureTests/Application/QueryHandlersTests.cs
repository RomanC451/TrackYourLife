using MediatR;
using NetArchTest.Rules;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.ArchitectureTests.Application;

public class QueryHandlersTests : BaseArchitectureTest
{
    private static IEnumerable<Type> CommandHandlerTypes =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .AreNotInterfaces()
            .And()
            .HaveNameEndingWith("QueryHandler")
            .GetTypes();

    [Fact]
    public void QueryHandlers_ShouldBeInternal() => ShouldBeInternal(CommandHandlerTypes);

    [Fact]
    public void QueryHandlers_ShouldBeSealed() => ShouldBeSealed(CommandHandlerTypes);

    [Fact]
    public void QueryHandlers_ShouldHaveQueryHandlerPostfix() =>
        ShouldHavePostfix(CommandHandlerTypes, "QueryHandler");
}
