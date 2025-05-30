﻿using MediatR;
using NetArchTest.Rules;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.ArchitectureTests.Application;

public class CommandHandlersTests : BaseArchitectureTest
{
    private static IEnumerable<Type> CommandHandlerTypes =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .AreNotInterfaces()
            .And()
            .HaveNameEndingWith("CommandHandler")
            .GetTypes();

    [Fact]
    public void CommandHandlers_ShouldBeInternal() => ShouldBeInternal(CommandHandlerTypes);

    [Fact]
    public void CommandHandlers_ShouldBeSealed() => ShouldBeSealed(CommandHandlerTypes);

    [Fact]
    public void CommandHandlers_ShouldHaveCommandHandlerPostfix() =>
        ShouldHavePostfix(CommandHandlerTypes, "CommandHandler");
}
