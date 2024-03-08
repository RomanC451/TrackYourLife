using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public sealed record LoginUserResponse(UserId UserId, string JwtToken);
