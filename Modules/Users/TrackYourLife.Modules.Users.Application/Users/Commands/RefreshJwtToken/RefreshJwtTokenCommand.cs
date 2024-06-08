using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.Users;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.RefreshJwtToken;

public sealed record RefreshJwtTokenCommand() : ICommand<TokenResponse>;
