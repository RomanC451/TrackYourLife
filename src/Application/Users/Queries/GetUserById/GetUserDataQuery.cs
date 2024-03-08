using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Queries.GetUserData;

public sealed record GetUserDataQuery() : IQuery<GetUserDataResult>;
