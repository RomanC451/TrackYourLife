using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.SearchedFoods.Specifications;

internal sealed class SearchedFoodWithNameSpecification(string name)
    : Specification<SearchedFood, SearchedFoodId>
{
    private readonly string _name = name.ToLower();

    public override Expression<Func<SearchedFood, bool>> ToExpression() =>
        food => food.Name == _name;
}
