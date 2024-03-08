using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using Microsoft.FeatureManagement;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Users.ValueObjects;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Application.Abstractions.Services;

namespace TrackYourLifeDotnet.Application.Users.Commands.Register;

public sealed class RegisterUserCommandHandler
    : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeatureManager _featureManager;

    public RegisterUserCommandHandler(
        IUserRepository memberRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IFeatureManager featureManager
    )
    {
        _userRepository = memberRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _featureManager = featureManager;
    }

    public async Task<Result<RegisterUserResult>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<Name> firstNameResult = Name.Create(command.FirstName);
        if (firstNameResult.IsFailure)
            return Result.Failure<RegisterUserResult>(firstNameResult.Error);

        Result<Name> lastNameResult = Name.Create(command.LastName);
        if (lastNameResult.IsFailure)
            return Result.Failure<RegisterUserResult>(lastNameResult.Error);

        Result<Email> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
            return Result.Failure<RegisterUserResult>(emailResult.Error);
        else if (!await _userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
            return Result.Failure<RegisterUserResult>(DomainErrors.Email.AlreadyUsed);

        Result<Password> passwordResult = Password.Create(command.Password);
        if (passwordResult.IsFailure)
            return Result.Failure<RegisterUserResult>(passwordResult.Error);

        HashedPassword hashedPassword = _passwordHasher.Hash(command.Password);

        User user = User.Create(
            new UserId(Guid.NewGuid()),
            emailResult.Value,
            hashedPassword,
            firstNameResult.Value,
            lastNameResult.Value
        );

        if (await _featureManager.IsEnabledAsync(FeatureFlags.SkipEmailVerification))
        {
            user.VerfiedOnUtc = DateTime.UtcNow;
        }

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RegisterUserResult(user.Id);

        return Result.Success(response);
    }
}
