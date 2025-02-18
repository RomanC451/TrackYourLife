using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler(
    IUserRepository userRepository,
    ITokenRepository userTokenRepository
) : ICommandHandler<VerifyEmailCommand>
{
    public async Task<Result> Handle(
        VerifyEmailCommand command,
        CancellationToken cancellationToken
    )
    {
        var emailVerificationToken = await userTokenRepository.GetByValueAsync(
            command.VerificationToken,
            cancellationToken
        );

        if (emailVerificationToken is null)
        {
            return Result.Failure(UserErrors.EmailVerificationToken.Invalid);
        }

        User? user = await userRepository.GetByIdAsync(
            emailVerificationToken.UserId,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(emailVerificationToken.UserId));
        }

        user.VerifyEmail();

        userTokenRepository.Remove(emailVerificationToken);

        return Result.Success();
    }
}
