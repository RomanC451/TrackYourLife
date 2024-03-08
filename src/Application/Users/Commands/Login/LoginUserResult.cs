using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public sealed record LoginUserResult(UserId UserId, string JwtToken);
