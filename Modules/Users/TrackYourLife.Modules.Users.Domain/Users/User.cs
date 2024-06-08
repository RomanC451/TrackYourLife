
using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Modules.Users.Domain.Users.DomainEvents;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Domain.Users;

public class User : AggregateRoot<UserId>, IAuditableEntity
{
    private User(UserId id, Email email, HashedPassword passwordHash, Name firstName, Name lastName)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
    }

    public Name FirstName { get; private set; }

    public Name LastName { get; private set; }

    public Email Email { get; private set; }

    public HashedPassword PasswordHash { get; private set; }

    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedOnUtc { get; set; }

    public DateTime? VerifiedOnUtc { get; set; }

    public static User Create(
        UserId id,
        Email email,
        HashedPassword password,
        Name firstName,
        Name lastName
    )
    {
        User user = new(id, email, password, firstName, lastName);

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
