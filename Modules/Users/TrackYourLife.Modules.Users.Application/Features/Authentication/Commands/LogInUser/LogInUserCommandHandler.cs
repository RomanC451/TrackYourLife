using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;

public sealed class LogInUserCommandHandler(
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
        Result<Email> emailResult = Email.Create(request.Email);

        Result<Password> passwordResult = Password.Create(request.Password);

        if (emailResult.IsFailure || passwordResult.IsFailure)
        {
            return Result.Failure<(string, Token)>(UserErrors.InvalidCredentials);
        }

        var user = await userQuery.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, passwordResult.Value.Value))
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
