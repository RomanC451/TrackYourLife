using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Repositories;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
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
        var firstNameResult = Name.Create(command.FirstName);
        var lastNameResult = Name.Create(command.LastName);
        var emailResult = Email.Create(command.Email);
        var passwordResult = Password.Create(command.Password);

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

        var result = User.Create(
            UserId.NewId(),
            emailResult.Value,
            hashedPassword,
            firstNameResult.Value,
            lastNameResult.Value
        );

        if (result.IsFailure)
            return Result.Failure(result.Error);

        User user = result.Value;

        if (featureManager.SkipEmailVerification)
        {
            user.VerifyEmail();
        }

        await memberRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(user.Id));
    }
}
