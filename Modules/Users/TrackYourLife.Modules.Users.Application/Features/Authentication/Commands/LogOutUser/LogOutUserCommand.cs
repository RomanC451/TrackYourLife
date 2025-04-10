using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogOutUser;

public sealed record LogOutUserCommand(DeviceId DeviceId, bool LogOutAllDevices) : ICommand;
