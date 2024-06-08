using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users.StrongTypes;
using TrackYourLife.Common.Domain.Users.Repositories;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.Remove;

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
        Result<UserId> userIdResult = _authService.GetUserIdFromJwtToken();

        if (userIdResult.IsFailure)
        {
            return Result.Failure(userIdResult.Error);
        }

        var userId = userIdResult.Value;

        Domain.Users.User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(DomainErrors.User.NotFound(userId));
        }

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
