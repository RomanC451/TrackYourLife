namespace TrackYourLifeDotnet.Application.Users.Commands.Update;

public sealed record UpdateUserResponse(Guid Id, string Email, string FirstName, string LastName);
