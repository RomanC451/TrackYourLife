using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;

public sealed record UpdateUserCommand(string FirstName, string LastName) : ICommand;
