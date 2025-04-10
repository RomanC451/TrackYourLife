using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Contracts.Users;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;

public sealed record RefreshJwtTokenCommand(string RefreshTokenValue, DeviceId DeviceId)
    : ICommand<(TokenResponse, Token)>;
