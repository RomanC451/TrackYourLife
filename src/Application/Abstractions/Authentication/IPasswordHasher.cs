using TrackYourLifeDotnet.Domain.Users.ValueObjects;

namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IPasswordHasher
{
    HashedPassword Hash(string password);

    bool Verify(string passwordHash, string inputPassword);
}
