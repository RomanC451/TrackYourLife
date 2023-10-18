using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public sealed record LoginUserResponse(Guid UserId, string JwtToken, UserToken RefreshToken);
