using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Contracts.Shared;
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
        var firstName = Name.Create(command.FirstName).Value;
        var lastName = Name.Create(command.LastName).Value;
        var email = Email.Create(command.Email).Value;
        var password = Password.Create(command.Password).Value;

        if (!await memberRepository.IsEmailUniqueAsync(email, cancellationToken))
            return Result.Failure(UserErrors.Email.AlreadyUsed);

        HashedPassword hashedPassword = passwordHasher.Hash(password.Value);

        var result = User.Create(UserId.NewId(), email, hashedPassword, firstName, lastName);

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
