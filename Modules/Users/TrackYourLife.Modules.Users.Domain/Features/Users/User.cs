using TrackYourLife.Modules.Users.Domain.Features.Users.DomainEvents;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
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
}
