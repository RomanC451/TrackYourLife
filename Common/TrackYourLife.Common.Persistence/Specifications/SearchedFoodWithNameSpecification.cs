using System.Linq.Expressions;
using TrackYourLife.Common.Domain.Foods;

namespace TrackYourLife.Common.Persistence.Specifications;

internal class SearchedFoodWithNameSpecification : Specification<SearchedFood, SearchedFoodId>
{
    private readonly string _name;

    public SearchedFoodWithNameSpecification(string name) => _name = name;

    public override Expression<Func<SearchedFood, bool>> ToExpression() =>
        food => food.Name == _name;
}
