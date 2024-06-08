using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

public interface IPasswordHasher
{
    HashedPassword Hash(string password);

    bool Verify(string passwordHash, string inputPassword);
}
