namespace TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;

public interface IFoodApiAuthDataStore
{
    public string GetAccessToken();

    public string GetUserId();

    public Result RefreshAuthDataAsync(string apiResponse);
}
