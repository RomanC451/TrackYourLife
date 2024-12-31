
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface IEmailService
{
    public Result SendVerificationEmail(string userEmail, string verificationLink);
}
