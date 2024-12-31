

using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Options;

/// <summary>
/// Represents the options for the Food API.
/// </summary>
public class FoodApiOptions : IOptions
{
    /// <summary>
    /// The configuration section name for the Food API options.
    /// </summary>
    public const string ConfigurationSection = "FoodApi";

    /// <summary>
    /// The base URL for the Food API.
    /// </summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// The base API URL for the Food API.
    /// </summary>
    public string BaseApiUrl { get; init; } = string.Empty;

    /// <summary>
    /// The search path for the Food API.
    /// </summary>
    public string SearchPath { get; init; } = string.Empty;

    /// <summary>
    /// The authentication token path for the Food API.
    /// </summary>
    public string AuthTokenPath { get; init; } = string.Empty;

    /// <summary>
    /// The cookie domains for the Food API.
    /// </summary>
    public string[] CookieDomains { get; init; } = Array.Empty<string>();

    /// <summary>
    /// The space encoded value for the Food API.
    /// </summary>
    public string SpaceEncoded { get; init; } = string.Empty;
}
