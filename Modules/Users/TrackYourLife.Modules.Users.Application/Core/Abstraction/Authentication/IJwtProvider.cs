using TrackYourLife.Modules.Users.Domain.Features.Users;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

public interface IJwtProvider
{
    string Generate(UserReadModel user);
}
