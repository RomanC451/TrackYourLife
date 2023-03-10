using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.Register;

public record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
    : ICommand<RegisterUserResponse>;
