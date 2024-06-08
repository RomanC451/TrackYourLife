using System.ComponentModel.DataAnnotations;

namespace TrackYourLife.Common.Domain.Shared;

public class DateOnlyAttribute : ValidationAttribute
{
    protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        if (value is string str && DateOnly.TryParse(str, out _))
        {
            return System.ComponentModel.DataAnnotations.ValidationResult.Success!;
        }

        return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid date format");
    }
}
