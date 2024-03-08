namespace TrackYourLifeDotnet.Domain.Foods;

public class NutritionalContent
{
    public double Calcium { get; set; }
    public double Carbohydrates { get; set; }
    public double Cholesterol { get; set; }
    public double Fat { get; set; }
    public double Fiber { get; set; }
    public double Iron { get; set; }
    public double MonounsaturatedFat { get; set; }
    public double NetCarbs { get; set; }
    public double PolyunsaturatedFat { get; set; }
    public double Potassium { get; set; }
    public double Protein { get; set; }
    public double SaturatedFat { get; set; }
    public double Sodium { get; set; }
    public double Sugar { get; set; }
    public double TransFat { get; set; }
    public double VitaminA { get; set; }
    public double VitaminC { get; set; }
    public Energy Energy { get; set; } = new();
}
