using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Application.Users.Commands.ResendVerificationEmail;

public sealed class ResendEmailVerificationComandHandler : ICommandHandler<ResendEmailVerificationCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;

    public ResendEmailVerificationComandHandler(IUserRepository userRepository, IUserTokenRepository userTokenRepository, IAuthService authService, IEmailService emailService)
    {
        _userRepository = userRepository;
        _userTokenRepository = userTokenRepository;
        _authService = authService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        Result<Email> emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure(DomainErrors.Email.InvalidFormat);
        }

        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
        {
            return Result.Failure(DomainErrors.Email.NotFound);
        }
        if (user.VerfiedOnUtc != null)
        {
            return Result.Failure(DomainErrors.Email.AlreadyVerified);
        }

        string emailVerificationLink = await _authService.GenerateEmailVerificationLink(
            user.Id,
            cancellationToken
        );

        await _emailService.SendVerifitionEmail(user.Email, emailVerificationLink);

        return Result.Success();
    }
}