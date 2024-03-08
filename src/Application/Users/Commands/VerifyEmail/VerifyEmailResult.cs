using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Commands.VerifyEmail;

public sealed record VerifyEmailResult(UserId UserId);
