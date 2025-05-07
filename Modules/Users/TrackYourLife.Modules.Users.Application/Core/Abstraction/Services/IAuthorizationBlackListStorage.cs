using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface IAuthorizationBlackListStorage
{
    void Add(LoggedOutUser user);
    bool Contains(string token);
    void Remove(UserId userId, DeviceId deviceId);
}
