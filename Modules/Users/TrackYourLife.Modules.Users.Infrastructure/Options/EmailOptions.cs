using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Users.Infrastructure.Options;

public class EmailOptions : IOptions
{
    public const string ConfigurationSection = "EmailHost";
    public string SenderEmail { get; init; } = string.Empty;

    public string SmtpHost { get; init; } = string.Empty;

    public int SmtpPort { get; init; } = 0;

    public string SmtpPassword { get; init; } = string.Empty;
}
