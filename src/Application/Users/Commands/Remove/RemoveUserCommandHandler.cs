using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.Users.Commands.Remove;

public sealed class RemoveUserCommandHandler : ICommandHandler<RemoveUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;

    public RemoveUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IAuthService authService
    )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    public async Task<Result> Handle(RemoveUserCommand _, CancellationToken cancellationToken)
    {
        Result<Guid> jwtResult = _authService.GetUserIdFromJwtToken();

        if (jwtResult.IsFailure)
        {
            return Result.Failure(jwtResult.Error);
        }

        var userId = new UserId(jwtResult.Value);

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(DomainErrors.User.NotFound(userId));
        }

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
