using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Application.Foods.Queries.GetFoodById;

public sealed record GetFoodByIdQuery(FoodId FoodId) : IQuery<GetFoodByIdResult>;
