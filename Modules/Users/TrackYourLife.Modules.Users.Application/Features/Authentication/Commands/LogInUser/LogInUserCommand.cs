using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;

public sealed record LogInUserCommand(string Email, string Password, DeviceId DeviceId)
    : ICommand<(string, Token)>;
