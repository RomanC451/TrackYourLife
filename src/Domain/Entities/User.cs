using TrackYourLifeDotnet.Domain.DomainEvents;
using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Domain.Entities;

public class User : AggregateRoot, IAuditableEntity
{
    private User(Guid id, Email email, HashedPassword password, Name firstName, Name lastName)
        : base(id)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
    }

    public Name FirstName { get; private set; }

    public Name LastName { get; private set; }

    public Email Email { get; private set; }

    public HashedPassword Password { get; private set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime? ModifiedOnUtc { get; set; }

    public virtual RefreshToken? RefreshToken { get; set; }

    public static User Create(
        Guid id,
        Email email,
        HashedPassword password,
        Name firstName,
        Name lastName
    )
    {
        User user = new(id, email, password, firstName, lastName);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(Guid.NewGuid(), user.Id));

        return user;
    }

    public void ChangeName(Name firstName, Name lastName)
    {
        if (!FirstName.Equals(firstName) || !LastName.Equals(lastName))
        {
            RaiseDomainEvent(new UserNameChangedDomainEvent(Guid.NewGuid(), Id));
        }

        FirstName = firstName;
        LastName = lastName;
    }

    public void ChangeEmail(Email email)
    {
        if (!Email.Equals(email))
        {
            RaiseDomainEvent(new UserEmailChangedDomainEvent(Guid.NewGuid(), Id));
        }

        Email = email;
    }

    public void ChangePassword(HashedPassword password)
    {
        if (!Password.Equals(password))
        {
            RaiseDomainEvent(new UserPasswordChangedDomainEvent(Guid.NewGuid(), Id));
        }

        Password = password;
    }
}
