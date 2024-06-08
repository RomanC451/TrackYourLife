using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.Foods;
using TrackYourLife.Common.Domain.Foods;

namespace TrackYourLife.Common.Application.Foods.Queries.GetFoodById;

public sealed record GetFoodByIdQuery(FoodId FoodId) : IQuery<FoodResponse>;
