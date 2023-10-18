using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Application.Abstractions.Services;

public interface IEmailService
{
    public Task SendVerifitionEmail(Email userEmail, string verificationLink);
   
}