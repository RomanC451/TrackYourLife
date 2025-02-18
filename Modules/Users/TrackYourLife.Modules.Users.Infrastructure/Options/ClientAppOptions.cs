using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Users.Infrastructure.Options;

public class ClientAppOptions : IOptions
{
    public const string ConfigurationSection = "ClientApp";

    public string BaseUrl { get; init; } = string.Empty;

    public string EmailVerificationPath { get; init; } = string.Empty;
}
