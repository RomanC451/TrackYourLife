using FluentValidation;
using TrackYourLifeDotnet.Infrastructure.Options;

namespace TrackYourLifeDotnet.Infrastructure.Validators;

public class UnitConversionOptionsValidator : AbstractValidator<UnitConversionOptions>
{
    public UnitConversionOptionsValidator()
    {
        RuleForEach(options => options.Mass)
            .ChildRules(unit =>
            {
                unit.RuleFor(u => u.Name).NotEmpty();
                unit.RuleForEach(u => u.ConversionFactors).NotEmpty();
            });

        RuleForEach(options => options.Volume)
            .ChildRules(unit =>
            {
                unit.RuleFor(u => u.Name).NotEmpty();
                unit.RuleForEach(u => u.ConversionFactors).NotEmpty();
            });
    }
}
