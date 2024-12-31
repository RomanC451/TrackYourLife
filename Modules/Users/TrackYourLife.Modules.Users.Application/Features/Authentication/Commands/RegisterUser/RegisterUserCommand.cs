using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand;
