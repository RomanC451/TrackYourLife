using TrackYourLife.Modules.Users.Domain.Users.DomainEvents;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Users;

public sealed class User : AggregateRoot<UserId>, IAuditableEntity
{
    public Name FirstName { get; private set; } = null!;

    public Name LastName { get; private set; } = null!;

    public Email Email { get; private set; } = null!;

    public HashedPassword PasswordHash { get; private set; } = null!;

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedOnUtc { get; set; }

    public DateTime? VerifiedOnUtc { get; set; }

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

    public static User Create(
        UserId id,
        Email email,
        HashedPassword password,
        Name firstName,
        Name lastName
    )
    {
        User user = new(id, email, password, firstName, lastName, DateTime.UtcNow);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void ChangeName(Name firstName, Name lastName)
    {
        if (!FirstName.Equals(firstName) || !LastName.Equals(lastName))
        {
            RaiseDomainEvent(new UserNameChangedDomainEvent(Id));
        }

        FirstName = firstName;
        LastName = lastName;
    }

    public void ChangeEmail(Email email)
    {
        if (!Email.Equals(email))
        {
            RaiseDomainEvent(new UserEmailChangedDomainEvent(Id));
        }

        Email = email;
    }

    public void ChangePassword(HashedPassword password)
    {
        if (!PasswordHash.Equals(password))
        {
            RaiseDomainEvent(new UserPasswordChangedDomainEvent(Id));
        }

        PasswordHash = password;
    }

    public void VerifyEmail()
    {
        VerifiedOnUtc = DateTime.UtcNow;
    }
}
