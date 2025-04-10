using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface IAuthService
{
    Task<Result<(string, Token)>> RefreshUserAuthTokensAsync(
        UserReadModel user,
        DeviceId deviceId,
        CancellationToken cancellationToken
    );

    Task LogOutUserAsync(
        UserId userId,
        DeviceId deviceId,
        bool logOutAllDevices,
        CancellationToken cancellationToken
    );

    Task<Result<string>> GenerateEmailVerificationLinkAsync(
        UserId userId,
        CancellationToken cancellationToken
    );
}
