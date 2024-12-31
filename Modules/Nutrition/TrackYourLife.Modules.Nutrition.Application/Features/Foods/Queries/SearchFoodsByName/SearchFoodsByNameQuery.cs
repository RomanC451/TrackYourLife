using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;

/// <summary>
/// Represents a query to retrieve a list of foods based on search parameters.
/// </summary>
public sealed record SearchFoodsByNameQuery(string SearchParam, int Page, int PageSize)
    : IQuery<PagedList<FoodReadModel>>;
