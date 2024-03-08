using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.ValueObjects;

namespace TrackYourLifeDotnet.Application.Abstractions.Services;

public interface IEmailService
{
    public Result SendVerifitionEmail(Email userEmail, string verificationLink);
}
