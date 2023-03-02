namespace TrackYourLifeDotnet.Application.Abstractions.Authentication;

public interface IRefreshTokenProvider
{
    string Generate();
}
