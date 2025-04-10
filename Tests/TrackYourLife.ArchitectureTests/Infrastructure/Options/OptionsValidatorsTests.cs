using FluentValidation;
using NetArchTest.Rules;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.ArchitectureTests.Infrastructure.Options
{
    public class OptionsValidatorsTests : BaseArchitectureTest
    {
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
                    return optionsType.GetInterfaces().Contains(typeof(IOptions));
                });

        [Fact]
        public void OptionsValidators_ShouldBeSealed() => ShouldBeSealed(OptionsValidators);

        [Fact]
        public void OptionsValidators_ShouldBeInternal() => ShouldBeInternal(OptionsValidators);

        [Fact]
        public void OptionsValidators_ShouldHaveValidatorPostfix() =>
            ShouldHavePostfix(OptionsValidators, "Validator");

        [Fact]
        public void OptionsValidators_ShouldHaveAtLeastOneValidationRule() =>
            CustomTest(
                OptionsValidators,
                (t) =>
                {
                    var validator = (IValidator)Activator.CreateInstance(t)!;
                    return validator.CreateDescriptor().Rules.Any();
                }
            );

        [Fact]
        public void OptionsValidators_ShouldValidateIOptionsType() =>
            CustomTest(
                OptionsValidators,
                (t) =>
                {
                    if (t.BaseType is null)
                    {
                        return false;
                    }

                    var optionsType = t.BaseType.GetGenericArguments()[0];
                    return optionsType.GetInterfaces().Contains(typeof(IOptions));
                }
            );
    }
}
