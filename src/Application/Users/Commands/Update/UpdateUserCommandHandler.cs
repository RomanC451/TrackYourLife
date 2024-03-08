using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Users.ValueObjects;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Commands.Update;

public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IAuthService authService
    )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    public async Task<Result<UpdateUserResult>> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<Guid> jwtResult = _authService.GetUserIdFromJwtToken();

        if (jwtResult.IsFailure)
        {
            return Result.Failure<UpdateUserResult>(jwtResult.Error);
        }

        var userId = new UserId(jwtResult.Value);

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UpdateUserResult>(DomainErrors.User.NotFound(userId));
        }

        Result<Name> firstNameResult = Name.Create(command.FirstName);
        Result<Name> lastNameResult = Name.Create(command.LastName);

        if (firstNameResult.IsFailure)
            return Result.Failure<UpdateUserResult>(firstNameResult.Error);

        if (lastNameResult.IsFailure)
            return Result.Failure<UpdateUserResult>(lastNameResult.Error);

        user.ChangeName(firstNameResult.Value, lastNameResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateUserResult(
            user.Id,
            user.Email.Value,
            firstNameResult.Value.Value,
            lastNameResult.Value.Value
        );
    }
}
