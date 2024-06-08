using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Domain.Users;


namespace TrackYourLife.Common.Infrastructure.Authentication;

public class UserIdentifierProvider : IUserIdentifierProvider
{
    public UserId UserId { get; }

    public UserIdentifierProvider(IHttpContextAccessor httpContextAccessor)
    {
        string userIdClaim =
            (httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier))
            ?? throw new ArgumentException(
                "The user identifier claim is required.",
                nameof(httpContextAccessor)
            );

        UserId = new UserId(Guid.Parse(userIdClaim));
    }
}
