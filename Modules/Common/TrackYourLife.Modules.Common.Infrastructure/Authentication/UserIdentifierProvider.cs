using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Common.Infrastructure.Authentication;

internal sealed class UserIdentifierProvider : IUserIdentifierProvider
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

        UserId = UserId.Create(Guid.Parse(userIdClaim));
    }
}
