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

    public RemoveUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RemoveUserResponse>> Handle(
        RemoveUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<RemoveUserResponse>(DomainErrors.User.NotFound(command.UserId));
        }

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RemoveUserResponse(user.Id);
    }
}
