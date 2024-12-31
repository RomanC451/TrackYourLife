using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Repositories;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository memberRepository,
    IUsersUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    UsersFeatureManagement featureManager
) : ICommandHandler<RegisterUserCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

        if (!await memberRepository.IsEmailUniqueAsync(emailResult.Value, cancellationToken))
            return Result.Failure(UserErrors.Email.AlreadyUsed);

        HashedPassword hashedPassword = passwordHasher.Hash(command.Password);

        User user = User.Create(
            UserId.NewId(),
            emailResult.Value,
            hashedPassword,
            firstNameResult.Value,
            lastNameResult.Value
        );

        if (featureManager.SkipEmailVerification)
        {
            user.VerifiedOnUtc = DateTime.UtcNow;
        }

        await memberRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(user.Id));
    }
}
