using System.Reflection;
using FluentValidation;
using NetArchTest.Rules;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.ArchitectureTests.Infrastructure.Options;

public class OptionsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> OptionsTypes =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IOptions))
            .GetTypes();

    private static IEnumerable<Type> ValidatedOptions =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .And()
            .HaveNameEndingWith("OptionsValidator")
            .GetTypes()
            .Select(t => t.BaseType!.GetGenericArguments()[0]);

    private static IEnumerable<Type> OptionsValidators =>
        Types
            .InAssemblies(InfrastructureAssemblies.Assemblies)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .GetTypes()
            .Where(t =>
            {
                var baseType = t.BaseType;

                if (baseType is null)
                {
                    return false;
                }

                var optionsType = baseType.GetGenericArguments()[0];

                var interfaces = optionsType.GetInterfaces();
                Console.WriteLine(interfaces);

                return Array.Exists(interfaces, i => i == typeof(IOptions));
            });

    [Fact]
    public void Options_ShouldBeSealed() => ShouldBeSealed(OptionsTypes);

    [Fact]
    public void Options_ShouldBeInternal() => ShouldBeInternal(OptionsTypes);

    [Fact]
    public void Options_ShouldHaveOptionsPostfix() => ShouldHavePostfix(OptionsTypes, "Options");

    [Fact]
    public void Options_ShouldHaveValidator() =>
        CustomTest(OptionsTypes, (t) => ValidatedOptions.Contains(t));

    [Fact]
    public void AllOptionsProperties_ShouldHaveValidationRules() =>
        CustomTest(
            OptionsValidators,
            (t) =>
            {
                if (t.BaseType is null)
                {
                    return true;
                }
                Type optionsType = t.BaseType.GetGenericArguments()[0];
                var commandProperties = optionsType.GetProperties(
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
