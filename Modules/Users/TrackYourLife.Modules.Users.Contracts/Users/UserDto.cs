
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Contracts.Users;

public sealed record UserDto(UserId Id, string Email, string FirstName, string LastName);
