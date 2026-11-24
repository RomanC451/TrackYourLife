namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

public interface IPersonalAccessTokenHasher
{
    string GenerateRawToken();

    string Hash(string rawToken);

    string GetVisiblePrefix(string rawToken);
}
