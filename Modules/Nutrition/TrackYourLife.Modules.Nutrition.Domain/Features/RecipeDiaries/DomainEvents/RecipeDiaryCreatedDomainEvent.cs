using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;

public sealed record RecipeDiaryCreatedDomainEvent(
    UserId UserId,
    RecipeId RecipeId,
    DateOnly Date,
    float Quantity,
    ServingSizeId ServingSizeId
) : IDirectDomainEvent;
