using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

/// <summary>
/// Represents a query to get a food item by its ID.
/// </summary>
namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;

public sealed record GetFoodByIdQuery(FoodId FoodId) : IQuery<FoodDto>;
