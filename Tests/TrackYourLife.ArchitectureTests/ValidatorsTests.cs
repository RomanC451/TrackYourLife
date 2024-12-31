using FluentValidation;
using NetArchTest.Rules;
using TrackYourLife.ArchitectureTests.Infrastructure;

namespace TrackYourLife.ArchitectureTests;

public class ValidatorsTests : BaseArchitectureTest
{
    private static IEnumerable<Type> Validators =>
        Types.InAssemblies(AllAssemblies).That().Inherit(typeof(AbstractValidator<>)).GetTypes();

    [Fact]
    public void Validators_ShouldBeInternal() => ShouldBeInternal(Validators);

    [Fact]
    public void Validators_ShouldBeSealed() => ShouldBeSealed(Validators);

    [Fact]
    public void Validators_ShouldHaveValidatorPostfix() =>
        ShouldHavePostfix(Validators, "Validator");
}
