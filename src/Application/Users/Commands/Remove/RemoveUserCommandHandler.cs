using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Users.Commands.Remove;

public sealed class RemoveUserCommandHandler
    : ICommandHandler<RemoveUserCommand, RemoveUserResponse>
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

    public async Task<Result<RemoveUserResponse>> Handle(
        RemoveUserCommand command,
        CancellationToken cancellationToken
    )
    {
        Guid userId = _authService.GetUserIdFromJwtToken(command.JwtToken);

        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<RemoveUserResponse>(DomainErrors.User.NotFound(userId));
        }

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RemoveUserResponse(user.Id);
    }
}
