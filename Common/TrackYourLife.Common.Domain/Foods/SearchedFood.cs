using TrackYourLife.Common.Domain.Primitives;

namespace TrackYourLife.Common.Domain.Foods;

public class SearchedFood : AggregateRoot<SearchedFoodId>
{
    public SearchedFood(SearchedFoodId id, string name)
        : base(id)
    {
        Name = name;
        RaiseDomainEvent(new FoodSearchedDomainEvent(id, Name));
    }

    public string Name { get; set; } = string.Empty;
}
