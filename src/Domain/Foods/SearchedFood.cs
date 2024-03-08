using TrackYourLifeDotnet.Domain.Foods.DomainEvents;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.Foods;

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
