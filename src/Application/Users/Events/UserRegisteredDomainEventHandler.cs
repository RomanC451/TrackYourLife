using System.Web;
using MediatR;
using Microsoft.FeatureManagement;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.DomainEvents;
using TrackYourLifeDotnet.Domain.Repositories;

namespace TrackYourLifeDotnet.Application.Users.Events;

internal sealed class UserRegisteredDomainEventHandler
    : INotificationHandler<UserRegisteredDomainEvent>
{

    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;

    private readonly IFeatureManager _featureManager;
    private readonly IUnitOfWork _unitOfWork;

    public UserRegisteredDomainEventHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        IAuthService authService,
        IFeatureManager featureManager,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _authService = authService;
        _featureManager = featureManager;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UserRegisteredDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        if (await _featureManager.IsEnabledAsync(FeatureFlags.SkipEmailVerification))
        {
            return;
        }

        string emailVerificationLink = await _authService.GenerateEmailVerificationLink(
            user.Id,
            cancellationToken
        );

        await _emailService.SendVerifitionEmail(user.Email, emailVerificationLink);
    }
}
