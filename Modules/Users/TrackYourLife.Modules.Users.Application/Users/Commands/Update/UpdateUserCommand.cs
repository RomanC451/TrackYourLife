using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Contracts.Users;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.Update;

public sealed record UpdateUserCommand(string FirstName, string LastName) : ICommand;
