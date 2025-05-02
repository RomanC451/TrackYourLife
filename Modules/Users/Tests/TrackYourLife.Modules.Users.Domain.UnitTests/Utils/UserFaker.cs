using Bogus;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Utils;

public static class UserFaker
{
    private static readonly Faker f = new();

    public static User Generate(
        UserId? id = null,
        Email? email = null,
        HashedPassword? password = null,
        Name? firstName = null,
        Name? lastName = null,
        DateTime? verifiedOnUtc = null
    )
    {
        return User.Create(
            id ?? UserId.NewId(),
            email ?? Email.Create(f.Internet.Email()).Value,
            password ?? new HashedPassword(f.Internet.Password()),
            firstName ?? Name.Create(f.Name.FirstName()).Value,
            lastName ?? Name.Create(f.Name.LastName()).Value
        ).Value;
    }

    public static UserReadModel GenerateReadModel(
        UserId? id = null,
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        string? passwordHash = null,
        DateTime? verifiedOnUtc = null
    )
    {
        return new UserReadModel(
            id ?? UserId.NewId(),
            firstName ?? f.Name.FirstName(),
            lastName ?? f.Name.LastName(),
            email ?? f.Internet.Email(),
            passwordHash ?? f.Internet.Password(),
            verifiedOnUtc
        );
    }
}
