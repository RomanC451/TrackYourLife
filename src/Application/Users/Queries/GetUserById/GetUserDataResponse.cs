using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Queries;

public sealed record GetUserDataResponse(
    UserId Id,
    string Email,
    string FirstName,
    string LastName
);
