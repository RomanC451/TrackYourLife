using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;

public sealed record RefreshJwtTokenResponse(string NewJwtToken, RefreshToken NewRefreshToken);
