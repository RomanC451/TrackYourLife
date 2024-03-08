namespace TrackYourLifeDotnet.Infrastructure.Services.FoodApiService;

public sealed record FoodApiAuthData(
    string TokenType,
    string AccessToken,
    int ExpiresIn,
    string RefreshToken,
    string UserId
);
