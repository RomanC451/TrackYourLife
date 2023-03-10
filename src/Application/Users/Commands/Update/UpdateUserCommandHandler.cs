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

    public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateUserResponse>> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UpdateUserResponse>(DomainErrors.User.NotFound(command.Id));
        }

        Result<FirstName> firstNameResult = FirstName.Create(command.FirstName);
        Result<LastName> lastNameResult = LastName.Create(command.LastName);

        if (firstNameResult.IsFailure)
        {
            return Result.Failure<UpdateUserResponse>(firstNameResult.Error);
        }

        if (lastNameResult.IsFailure)
        {
            return Result.Failure<UpdateUserResponse>(lastNameResult.Error);
        }

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
