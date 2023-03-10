using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Users.Commands.Register;

public record RegisterUserResponse(Guid UserId, string JwtToken, RefreshToken RefreshToken);
