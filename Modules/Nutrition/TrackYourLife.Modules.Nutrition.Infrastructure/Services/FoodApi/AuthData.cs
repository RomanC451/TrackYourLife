namespace TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;

internal class AuthData
{
    public string TokenType { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } = 0;
    public string RefreshToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
