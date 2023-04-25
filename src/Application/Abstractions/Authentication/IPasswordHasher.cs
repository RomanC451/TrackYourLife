using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IPasswordHasher
{
    HashedPassword Hash(string password);

    bool Verify(string passwordHash, string inputPassword);
}
