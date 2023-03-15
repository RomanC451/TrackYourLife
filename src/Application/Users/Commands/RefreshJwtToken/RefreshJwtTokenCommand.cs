using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;

public sealed record RefreshJwtTokenCommand(string? RefreshToken)
    : ICommand<RefreshJwtTokenResponse>;
