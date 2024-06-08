using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface IEmailService
{
    public Result SendVerificationEmail(Email userEmail, string verificationLink);
}
