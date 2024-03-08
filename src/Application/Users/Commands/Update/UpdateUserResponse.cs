using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Commands.Update;

public sealed record UpdateUserResponse(
    UserId UserId,
    string Email,
    string FirstName,
    string LastName
);
