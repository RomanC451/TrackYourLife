using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

internal sealed class AuthorizationBlackListStorage : IAuthorizationBlackListStorage
{
    private readonly HashSet<LoggedOutUser> _blackList = [];

    public void Add(LoggedOutUser user) => _blackList.Add(item: user);

    public bool Contains(string token) => _blackList.Any(user => user.Token == token);

    public void Remove(UserId userId, DeviceId deviceId) =>
        _blackList.RemoveWhere(user => user.UserId == userId && user.DeviceId == deviceId);
}
