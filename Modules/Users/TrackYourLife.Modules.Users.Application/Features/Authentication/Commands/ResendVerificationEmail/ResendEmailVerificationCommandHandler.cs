using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;

internal sealed class ResendEmailVerificationCommandHandler(
    IUserQuery userQuery,
    IAuthService authService,
    IEmailService emailService
) : ICommandHandler<ResendEmailVerificationCommand>
{
    public async Task<Result> Handle(
        ResendEmailVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure(UserErrors.Email.InvalidFormat);
        }

        var user = await userQuery.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.Email.EmailNotFound);
        }
        if (user.VerifiedOnUtc != null)
        {
            return Result.Failure(UserErrors.Email.AlreadyVerified);
        }

        var linkGeneratorResult = await authService.GenerateEmailVerificationLinkAsync(
            user.Id,
            cancellationToken
        );

        if (linkGeneratorResult.IsFailure)
        {
            return Result.Failure(linkGeneratorResult.Error);
        }

        string emailVerificationLink = linkGeneratorResult.Value;

        emailService.SendVerificationEmail(user.Email, emailVerificationLink);

        return Result.Success();
    }
}
