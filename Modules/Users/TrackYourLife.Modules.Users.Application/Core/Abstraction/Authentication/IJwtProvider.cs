using TrackYourLife.Modules.Users.Domain.Users;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

public interface IJwtProvider
{
    string Generate(UserReadModel user);
}
