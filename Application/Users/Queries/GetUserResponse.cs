using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Application.Users.Queries;

public sealed record GetUserResponse(Guid Id, string Email, string FirstName, string LastName);
