using TrackYourLife.Common.Domain.DomainEvents;

namespace TrackYourLife.Common.Domain.Foods;

public sealed record FoodSearchedDomainEvent(SearchedFoodId Id, string FoodName)
    : DomainEvent<SearchedFoodId>(Id);
