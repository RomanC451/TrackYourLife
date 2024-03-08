namespace TrackYourLifeDotnet.Infrastructure.Options;

public class UnitConversionOptions
{
    public const string ConfigurationSection = "UnitConversion";
    public List<Unit> Mass { get; set; } = new();
    public List<Unit> Volume { get; set; } = new();

    public class Unit
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Aliases { get; set; } = new();
        public Dictionary<string, double> ConversionFactors { get; set; } = new();
    }
}
