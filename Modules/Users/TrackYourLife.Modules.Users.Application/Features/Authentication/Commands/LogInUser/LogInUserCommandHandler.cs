using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;

internal sealed class LogInUserCommandHandler(
    IUserQuery userQuery,
    IPasswordHasher passwordHasher,
    IAuthService authService
) : ICommandHandler<LogInUserCommand, (string, Token)>
{
    public async Task<Result<(string, Token)>> Handle(
        LogInUserCommand request,
        CancellationToken cancellationToken
    )
    {
        Email email = Email.Create(request.Email).Value;

        Password password = Password.Create(request.Password).Value;

        var user = await userQuery.GetByEmailAsync(email, cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, password.Value))
        {
            return Result.Failure<(string, Token)>(UserErrors.InvalidCredentials);
        }

        if (user.VerifiedOnUtc == null)
        {
            return Result.Failure<(string, Token)>(UserErrors.Email.NotVerified);
        }

        var refreshAuthTokensResult = await authService.RefreshUserAuthTokensAsync(
            user,
            request.DeviceId,
            cancellationToken
        );

        if (refreshAuthTokensResult.IsFailure)
        {
            return Result.Failure<(string, Token)>(refreshAuthTokensResult.Error);
        }

        (string jwtToken, Token refreshToken) = refreshAuthTokensResult.Value;

        return Result.Success((jwtToken, refreshToken));
    }
}
