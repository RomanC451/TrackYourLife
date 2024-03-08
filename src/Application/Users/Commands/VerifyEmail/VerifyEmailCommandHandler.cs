using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.Repositories;

namespace TrackYourLifeDotnet.Application.Users.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler
    : ICommandHandler<VerifyEmailCommand, VerifyEmailResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailCommandHandler(
        IUserRepository userRepository,
        IUserTokenRepository userTokenRepository,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<VerifyEmailResult>> Handle(
        VerifyEmailCommand command,
        CancellationToken cancellationToken
    )
    {
        var emailVerificationToken = await _userTokenRepository.GetByValueAsync(
            command.VerificationToken,
            cancellationToken
        );

        if (emailVerificationToken is null)
        {
            return Result.Failure<VerifyEmailResult>(DomainErrors.EmailVerificationToken.Invalid);
        }

        User? user = await _userRepository.GetByIdAsync(
            emailVerificationToken.UserId,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure<VerifyEmailResult>(
                DomainErrors.User.NotFound(emailVerificationToken.UserId)
            );
        }

        user.VerifyEmail();

        _userTokenRepository.Remove(emailVerificationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new VerifyEmailResult(user.Id));
    }
}
