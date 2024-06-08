using TrackYourLife.Common.Application.Core.Abstractions.Messaging;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand;
