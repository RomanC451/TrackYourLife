namespace TrackYourLifeDotnet.Infrastructure.Options;

public class FoodApiOptions
{
    public const string ConfigurationSection = "FoodApi";

    public string BaseUrl { get; init; } = string.Empty;
    public string BaseApiUrl { get; init; } = string.Empty;

    public string SearchPath { get; init; } = string.Empty;

    public string LogInFormPath { get; init; } = string.Empty;

    public string LogInJsonPath { get; init; } = string.Empty;

    public string AuthTokenPath { get; init; } = string.Empty;
    public string[] CookieDoamins { get; init; } = Array.Empty<string>();

    public string SpaceEncoded { get; init; } = string.Empty;

    public int PageSize { get; init; }
}
