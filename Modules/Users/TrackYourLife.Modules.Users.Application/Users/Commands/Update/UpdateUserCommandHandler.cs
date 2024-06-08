using TrackYourLife.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users.StrongTypes;
using TrackYourLife.Common.Domain.Users;
using TrackYourLife.Common.Domain.Users.ValueObjects;
using TrackYourLife.Common.Domain.Users.Repositories;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Contracts.Users;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.Update;

public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        UserId userId = _userIdentifierProvider.UserId;

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(DomainErrors.User.NotFound(userId));
        }

        Result<Name> firstNameResult = Name.Create(command.FirstName);

        Result<Name> lastNameResult = Name.Create(command.LastName);

        if (firstNameResult.IsFailure)
            return Result.Failure<UserResponse>(firstNameResult.Error);

        if (lastNameResult.IsFailure)
            return Result.Failure<UserResponse>(lastNameResult.Error);

        user.ChangeName(firstNameResult.Value, lastNameResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
