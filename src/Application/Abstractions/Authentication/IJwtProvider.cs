using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string Generate(User userId);
}
