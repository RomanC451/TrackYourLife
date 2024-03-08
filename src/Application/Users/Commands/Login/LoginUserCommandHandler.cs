using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Users.ValueObjects;
using TrackYourLifeDotnet.Domain.Users;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAuthService authService
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _authService = authService;
    }

    public async Task<Result<LoginUserResult>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        Result<Email> emailResult = Email.Create(request.Email);

        Result<Password> passwordResult = Password.Create(request.Password);

        if (emailResult.IsFailure || passwordResult.IsFailure)
        {
            return Result.Failure<LoginUserResult>(DomainErrors.User.InvalidCredentials);
        }

        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (
            user is null || !_passwordHasher.Verify(user.Password.Value, passwordResult.Value.Value)
        )
        {
            return Result.Failure<LoginUserResult>(DomainErrors.User.InvalidCredentials);
        }

        if (user.VerfiedOnUtc == null)
        {
            return Result.Failure<LoginUserResult>(DomainErrors.Email.NotVerified);
        }

        (string jwtToken, UserToken refreshToken) = await _authService.RefreshUserAuthTokensAsync(
            user,
            cancellationToken
        );

        _authService.SetRefreshTokenCookie(refreshToken);

        return new LoginUserResult(user.Id, jwtToken);
    }
}
