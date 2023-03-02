namespace TrackYourLifeDotnet.Application.Users.Commands.Update;

public sealed record UpdateUserRequest(Guid Id, string FirstName, string LastName);
