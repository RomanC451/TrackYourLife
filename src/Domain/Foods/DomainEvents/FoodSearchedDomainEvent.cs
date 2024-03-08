using TrackYourLifeDotnet.Domain.DomainEvents;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Foods.DomainEvents;

public sealed record FoodSearchedDomainEvent(SearchedFoodId Id, string FoodName)
    : DomainEvent<SearchedFoodId>(Id);
