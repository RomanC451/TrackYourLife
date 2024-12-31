using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Users;

public sealed record UserReadModel(
    UserId Id,
    string FirstName,
    string LastName,
    string Email,
    string PasswordHash,
    DateTime? VerifiedOnUtc
) : IReadModel<UserId>;
