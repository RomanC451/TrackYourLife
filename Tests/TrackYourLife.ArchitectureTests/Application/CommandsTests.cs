using System.Reflection;
using FluentValidation;
using NetArchTest.Rules;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.ArchitectureTests.Application;

public class CommandsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> CommandTypes =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .GetTypes();

    private static IEnumerable<Type> ValidatedCommands =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .GetTypes()
            .Select(t => t.BaseType!.GetGenericArguments()[0]);

    private static IEnumerable<Type> Validators =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .GetTypes();

    [Fact]
    public void Commands_ShouldBeSealed() => ShouldBeSealed(CommandTypes);

    [Fact]
    public void Commands_ShouldBeDefinedAsRecords() => ShouldBeDefinedAsRecords(CommandTypes);

    [Fact]
    public void Commands_ShouldHaveCommandPostfix() => ShouldHavePostfix(CommandTypes, "Command");

    [Fact]
    public void Commands_ShouldHaveValidator() =>
        CustomTest(CommandTypes, (t) => ValidatedCommands.Contains(t));

    [Fact]
    public void AllCommandProperties_ShouldHaveValidationRules() =>
        CustomTest(
            Validators,
            (t) =>
            {
                Type commandType = t.BaseType!.GetGenericArguments()[0];
                var commandProperties = commandType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance
                );

                var validator = (IValidator)Activator.CreateInstance(t)!;

                foreach (var property in commandProperties)
                {
                    // Act
                    if (!validator.CreateDescriptor().GetRulesForMember(property.Name).Any())
                    {
                        return false;
                    }
                }
                return true;
            }
        );
}
