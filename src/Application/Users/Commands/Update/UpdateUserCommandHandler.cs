using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Application.Users.Commands.Update;

public sealed class UpdateUserCommandHandler
    : ICommandHandler<UpdateUserCommand, UpdateUserResponse>
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

    public async Task<Result<UpdateUserResponse>> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<Guid> jwtResult = _authService.GetUserIdFromJwtToken(command.JwtToken);

        if (jwtResult.IsFailure)
        {
            return Result.Failure<UpdateUserResponse>(jwtResult.Error);
        }

        User? user = await _userRepository.GetByIdAsync(jwtResult.Value, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UpdateUserResponse>(DomainErrors.User.NotFound(jwtResult.Value));
        }

        Result<Name> firstNameResult = Name.Create(command.FirstName);
        Result<Name> lastNameResult = Name.Create(command.LastName);

        if (firstNameResult.IsFailure)
            return Result.Failure<UpdateUserResponse>(firstNameResult.Error);

        if (lastNameResult.IsFailure)
            return Result.Failure<UpdateUserResponse>(lastNameResult.Error);

        user.ChangeName(firstNameResult.Value, lastNameResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateUserResponse(
            user.Id,
            user.Email.Value,
            firstNameResult.Value.Value,
            lastNameResult.Value.Value
        );
    }
}
