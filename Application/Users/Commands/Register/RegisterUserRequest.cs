namespace TrackYourLifeDotnet.Application.Users.Commands.Register;

public record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);
