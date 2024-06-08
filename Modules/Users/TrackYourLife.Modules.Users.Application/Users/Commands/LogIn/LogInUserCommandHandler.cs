using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users;
using TrackYourLife.Common.Domain.Users.Repositories;
using TrackYourLife.Common.Domain.Users.ValueObjects;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Contracts.Users;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.LogIn;

public sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, TokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthService _authService;

    public LogInUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthService authService
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authService = authService;
    }

    public async Task<Result<TokenResponse>> Handle(
        LogInUserCommand request,
        CancellationToken cancellationToken
    )
    {
        Result<Email> emailResult = Email.Create(request.Email);

        Result<Password> passwordResult = Password.Create(request.Password);

        if (emailResult.IsFailure || passwordResult.IsFailure)
        {
            return Result.Failure<TokenResponse>(DomainErrors.User.InvalidCredentials);
        }

        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (
            user is null
            || !_passwordHasher.Verify(user.PasswordHash.Value, passwordResult.Value.Value)
        )
        {
            return Result.Failure<TokenResponse>(DomainErrors.User.InvalidCredentials);
        }

        if (user.VerifiedOnUtc == null)
        {
            return Result.Failure<TokenResponse>(DomainErrors.Email.NotVerified);
        }

        (string jwtToken, UserToken refreshToken) = await _authService.RefreshUserAuthTokensAsync(
            user,
            cancellationToken
        );

        _authService.SetRefreshTokenCookie(refreshToken);

        return Result.Success(new TokenResponse(jwtToken));
    }
}
