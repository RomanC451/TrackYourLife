namespace TrackYourLife.Modules.Nutrition.Presentation.Contracts;

public static class ApiRoutes
{
    private const string Root = "api";
    public const string Foods = $"{Root}/foods";
    public const string NutritionDiaries = $"{Root}/nutrition-diaries";
    public const string FoodDiaries = $"{Root}/food-diaries";
    public const string RecipeDiaries = $"{Root}/recipe-diaries";
    public const string Recipes = $"{Root}/recipes";
    public const string DailyNutritionOverviews = $"{Root}/daily-nutrition-overviews";
}
