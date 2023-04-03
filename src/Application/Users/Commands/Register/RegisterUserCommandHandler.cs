using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Application.Users.Commands.Register;

public sealed class RegisterUserCommandHandler
    : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository memberRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IAuthService authService
    )
    {
        _userRepository = memberRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _authService = authService;
    }

    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<Email> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure<RegisterUserResponse>(emailResult.Error);
        else if (!await _userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
            return Result.Failure<RegisterUserResponse>(DomainErrors.Email.AlreadyUsed);

        Result<PasswordHash> hashedPasswordResult = PasswordHash.CreateHash(
            command.Password,
            _passwordHasher.Hash
        );
        if (hashedPasswordResult.IsFailure)
            return Result.Failure<RegisterUserResponse>(hashedPasswordResult.Error);

        Result<FirstName> firstNameResult = FirstName.Create(command.FirstName);
        if (firstNameResult.IsFailure)
            return Result.Failure<RegisterUserResponse>(firstNameResult.Error);

        Result<LastName> lastNameResult = LastName.Create(command.LastName);
        if (lastNameResult.IsFailure)
            return Result.Failure<RegisterUserResponse>(lastNameResult.Error);

        User user = User.Create(
            Guid.NewGuid(),
            emailResult.Value,
            hashedPasswordResult.Value,
            firstNameResult.Value,
            lastNameResult.Value
        );

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        (string jwtToken, RefreshToken refreshToken) = await _authService.RefreshUserAuthTokens(
            user,
            cancellationToken
        );

        _authService.SetRefreshTokenCookie(refreshToken);

        RegisterUserResponse response = new(user.Id, jwtToken, refreshToken);

        return Result.Success(response);
    }
}
