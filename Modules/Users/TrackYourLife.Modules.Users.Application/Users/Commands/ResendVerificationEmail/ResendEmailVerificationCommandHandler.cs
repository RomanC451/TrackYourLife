using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Users.Repositories;
using TrackYourLife.Common.Domain.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.ResendVerificationEmail;

public sealed class ResendEmailVerificationCommandHandler
    : ICommandHandler<ResendEmailVerificationCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;

    public ResendEmailVerificationCommandHandler(
        IUserRepository userRepository,
        IAuthService authService,
        IEmailService emailService
    )
    {
        _userRepository = userRepository;
        _authService = authService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(
        ResendEmailVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure(DomainErrors.Email.InvalidFormat);
        }

        Domain.Users.User? user = await _userRepository.GetByEmailAsync(
            emailResult.Value,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure(DomainErrors.Email.NotFound);
        }
        if (user.VerifiedOnUtc != null)
        {
            return Result.Failure(DomainErrors.Email.AlreadyVerified);
        }

        string emailVerificationLink = await _authService.GenerateEmailVerificationLinkAsync(
            user.Id,
            cancellationToken
        );

        _emailService.SendVerificationEmail(user.Email, emailVerificationLink);

        return Result.Success();
    }
}
