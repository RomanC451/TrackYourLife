using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserResponse>
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

    public async Task<Result<LoginUserResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        Result<Email> emailResult = Email.Create(request.Email);

        Result<Password> passwordResult = Password.Create(request.Password);

        if (emailResult.IsFailure || passwordResult.IsFailure)
        {
            return Result.Failure<LoginUserResponse>(DomainErrors.User.InvalidCredentials);
        }

        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (
            user is null || !_passwordHasher.Verify(user.Password.Value, passwordResult.Value.Value)
        )
        {
            return Result.Failure<LoginUserResponse>(DomainErrors.User.InvalidCredentials);
        }

        if (user.VerfiedOnUtc == null)
        {
            return Result.Failure<LoginUserResponse>(DomainErrors.Email.NotVerified);
        }

        (string jwtToken, UserToken refreshToken) = await _authService.RefreshUserAuthTokens(
            user,
            cancellationToken
        );

        _authService.SetRefreshTokenCookie(refreshToken);

        return new LoginUserResponse(user.Id, jwtToken, refreshToken);
    }
}
