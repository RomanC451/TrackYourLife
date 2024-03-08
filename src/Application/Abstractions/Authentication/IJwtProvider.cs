using TrackYourLifeDotnet.Domain.Users;

namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string Generate(User userId);
}
