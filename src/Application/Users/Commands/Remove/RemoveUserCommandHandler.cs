using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Entities;

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
        Result<Guid> jwtResult = _authService.GetUserIdFromJwtToken(command.JwtToken);

        if (jwtResult.IsFailure)
        {
            return Result.Failure<RemoveUserResponse>(jwtResult.Error);
        }

        User? user = await _userRepository.GetByIdAsync(jwtResult.Value, cancellationToken);

        if (user is null)
        {
            return Result.Failure<RemoveUserResponse>(DomainErrors.User.NotFound(jwtResult.Value));
        }

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RemoveUserResponse(user.Id);
    }
}
