using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.Users;

public record LoggedOutUser(string Token, UserId UserId, DeviceId DeviceId);
