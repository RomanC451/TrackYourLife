using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Contracts.Users;

public sealed record UserResponse(UserId UserId, string Email, string FirstName, string LastName);
