using System.Reflection;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

public sealed class NutritionalContent
{
    public float Calcium { get; set; }
    public float Carbohydrates { get; set; }
    public float Cholesterol { get; set; }
    public float Fat { get; set; }
    public float Fiber { get; set; }
    public float Iron { get; set; }
    public float MonounsaturatedFat { get; set; }
    public float NetCarbs { get; set; }
    public float PolyunsaturatedFat { get; set; }
    public float Potassium { get; set; }
    public float Protein { get; set; }
    public float SaturatedFat { get; set; }
    public float Sodium { get; set; }
    public float Sugar { get; set; }
    public float TransFat { get; set; }
    public float VitaminA { get; set; }
    public float VitaminC { get; set; }
    public Energy Energy { get; set; } = new();

    public NutritionalContent MultiplyNutritionalValues(float multiplier)
    {
        // Clone the current object
        var clone = (NutritionalContent)MemberwiseClone();

        // Multiply all float properties by the multiplier
        foreach (PropertyInfo property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(float))
            {
                var value = property.GetValue(clone);
                if (value != null)
                {
                    float newValue = (float)value * multiplier;
                    property.SetValue(clone, newValue);
                }
            }
        }

        // Multiply the Energy value
        clone.Energy = new Energy { Value = Energy.Value * multiplier };

        // Return the modified clone
        return clone;
    }

    public void AddNutritionalValues(NutritionalContent other)
    {
        foreach (PropertyInfo property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(float))
            {
                var currentValue = property.GetValue(this);
                var otherValue = property.GetValue(other);
                if (currentValue != null && otherValue != null)
                {
                    var newValue = (float)currentValue + (float)otherValue;

                    property.SetValue(this, newValue);
                }
            }
        }

        Energy.Value = Energy.Value + other.Energy.Value;
    }

    public void SubtractNutritionalValues(NutritionalContent other)
    {
        foreach (PropertyInfo property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(float))
            {
                var currentValue = property.GetValue(this);
                var otherValue = property.GetValue(other);
                if (currentValue != null && otherValue != null)
                {
                    var newValue = Math.Max(0, (float)currentValue - (float)otherValue);

                    property.SetValue(this, newValue);
                }
            }
        }

        Energy.Value = Math.Max(0, Energy.Value - other.Energy.Value);
    }
}
