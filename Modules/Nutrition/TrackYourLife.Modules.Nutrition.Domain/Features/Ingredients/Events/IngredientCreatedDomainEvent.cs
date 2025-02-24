using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients.Events;

public sealed record IngredientCreatedDomainEvent(UserId UserId, FoodId FoodId) : IDomainEvent;
