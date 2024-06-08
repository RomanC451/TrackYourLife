using Microsoft.FeatureManagement;
using TrackYourLife.Domain.Users.StrongTypes;
using TrackYourLife.Contracts.Common;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users;
using TrackYourLife.Common.Domain.Users.ValueObjects;
using TrackYourLife.Common.Domain.Users.Repositories;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.Register;

public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
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

    public async Task<Result> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<Name> firstNameResult = Name.Create(command.FirstName);
        Result<Name> lastNameResult = Name.Create(command.LastName);
        Result<Email> emailResult = Email.Create(command.Email);
        Result<Password> passwordResult = Password.Create(command.Password);

        Result firstFailureOrSuccess = Result.FirstFailureOrSuccess(
            firstNameResult,
            lastNameResult,
            emailResult,
            passwordResult
        );
        if (firstFailureOrSuccess.IsFailure)
            return Result.Failure(firstFailureOrSuccess.Error);

        if (!await _userRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
            return Result.Failure(DomainErrors.Email.AlreadyUsed);

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
            user.VerifiedOnUtc = DateTime.UtcNow;
        }

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(user.Id));
    }
}
