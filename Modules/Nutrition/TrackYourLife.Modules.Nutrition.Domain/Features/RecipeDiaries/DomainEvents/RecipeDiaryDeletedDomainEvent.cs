using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;

public sealed record RecipeDiaryDeletedDomainEvent(
    UserId UserId,
    DateOnly Date,
    RecipeId RecipeId,
    float Quantity
) : IDirectDomainEvent;
