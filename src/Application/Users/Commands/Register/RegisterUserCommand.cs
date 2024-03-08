using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<RegisterUserResult>;
