namespace TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;

public interface IYoutubeSettingsPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string inputPassword);
}
