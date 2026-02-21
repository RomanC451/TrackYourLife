using TrackYourLife.Modules.Users.Domain.Features.Users.DomainEvents;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Domain.Features.Users;

public sealed class User : AggregateRoot<UserId>, IAuditableEntity
{
    public Name FirstName { get; private set; } = null!;

    public Name LastName { get; private set; } = null!;

    public Email Email { get; private set; } = null!;

    public HashedPassword PasswordHash { get; private set; } = null!;

    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? ModifiedOnUtc { get; }

    public DateTime? VerifiedOnUtc { get; private set; }

    public PlanType PlanType { get; private set; } = PlanType.Free;

    public string? StripeCustomerId { get; private set; }

    public DateTime? SubscriptionEndsAtUtc { get; private set; }

    public SubscriptionStatus? SubscriptionStatus { get; private set; }

    /// <summary>
    /// When true, the user has requested cancellation and the subscription will not renew at period end.
    /// </summary>
    public bool SubscriptionCancelAtPeriodEnd { get; private set; }

    private User(
        UserId id,
        Email email,
        HashedPassword passwordHash,
        Name firstName,
        Name lastName,
        DateTime createdOnUtc
    )
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        CreatedOnUtc = createdOnUtc;
    }

    private User() { }

    public static Result<User> Create(
        UserId id,
        Email email,
        HashedPassword password,
        Name firstName,
        Name lastName
    )
    {
        User user = new(id, email, password, firstName, lastName, DateTime.UtcNow);

        user.RaiseOutboxDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void ChangeName(Name firstName, Name lastName)
    {
        if (!FirstName.Equals(firstName) || !LastName.Equals(lastName))
        {
            FirstName = firstName;
            LastName = lastName;
            RaiseOutboxDomainEvent(new UserNameChangedDomainEvent(Id));
        }
    }

    public void ChangeEmail(Email email)
    {
        if (!Email.Equals(email))
        {
            Email = email;
            RaiseOutboxDomainEvent(new UserEmailChangedDomainEvent(Id));
        }
    }

    public void ChangePassword(HashedPassword password)
    {
        if (!PasswordHash.Equals(password))
        {
            PasswordHash = password;
            RaiseOutboxDomainEvent(new UserPasswordChangedDomainEvent(Id));
        }
    }

    public void VerifyEmail()
    {
        VerifiedOnUtc = DateTime.UtcNow;
    }

    public void SetStripeCustomerId(string stripeCustomerId)
    {
        StripeCustomerId = stripeCustomerId;
    }

    public void SetProSubscription(
        string stripeCustomerId,
        DateTime periodEndUtc,
        SubscriptionStatus subscriptionStatus,
        bool cancelAtPeriodEnd = false
    )
    {
        StripeCustomerId = stripeCustomerId;
        SubscriptionEndsAtUtc = periodEndUtc;
        SubscriptionStatus = subscriptionStatus;
        SubscriptionCancelAtPeriodEnd = cancelAtPeriodEnd;
        PlanType = PlanType.Pro;
    }

    public void ClearProSubscription(SubscriptionStatus subscriptionStatus)
    {
        SubscriptionStatus = subscriptionStatus;
        PlanType = PlanType.Free;
        SubscriptionEndsAtUtc = null;
        SubscriptionCancelAtPeriodEnd = false;
        // Keep StripeCustomerId so we can reuse the same customer for future checkouts
    }

    public Result UpdateProSubscriptionPeriodEnd(
        DateTime periodEndUtc,
        SubscriptionStatus subscriptionStatus,
        bool cancelAtPeriodEnd
    )
    {
        if (PlanType != PlanType.Pro)
        {
            return Result.Failure(UserErrors.Subscription.NotPro);
        }

        SubscriptionEndsAtUtc = periodEndUtc;
        SubscriptionStatus = subscriptionStatus;
        SubscriptionCancelAtPeriodEnd = cancelAtPeriodEnd;
        return Result.Success();
    }
}
