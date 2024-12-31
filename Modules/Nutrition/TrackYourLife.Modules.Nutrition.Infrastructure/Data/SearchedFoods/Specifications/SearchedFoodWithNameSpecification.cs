using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.SearchedFoods.Specifications;

internal class SearchedFoodWithNameSpecification(string name)
    : Specification<SearchedFood, SearchedFoodId>
{
    public override Expression<Func<SearchedFood, bool>> ToExpression() =>
        food => food.Name == name;
}
