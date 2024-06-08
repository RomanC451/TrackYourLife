using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users.Repositories;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
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

    public async Task<Result> Handle(
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
            return Result.Failure(DomainErrors.EmailVerificationToken.Invalid);
        }

        Domain.Users.User? user = await _userRepository.GetByIdAsync(
            emailVerificationToken.UserId,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure(DomainErrors.User.NotFound(emailVerificationToken.UserId));
        }

        user.VerifyEmail();

        _userTokenRepository.Remove(emailVerificationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
