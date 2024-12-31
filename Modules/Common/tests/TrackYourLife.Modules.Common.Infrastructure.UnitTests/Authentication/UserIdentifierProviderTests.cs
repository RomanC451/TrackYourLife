using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Common.Infrastructure.Authentication;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Authentication;

public class UserIdentifierProviderTests : IDisposable
{
    private readonly IHttpContextAccessor httpContextAccessor =
        Substitute.For<IHttpContextAccessor>();

    [Fact]
    public void Constructor_WhenUserIdClaimIsPresent_ShouldSetUserId()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = new UserIdentifierProvider(httpContextAccessor);

        // Assert
        result.UserId.Should().Be(UserId.Create(Guid.Parse(userId)));
    }

    [Fact]
    public void Constructor_WhenUserIdClaimIsMissing_ShouldThrowArgumentException()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        Action act = () => new UserIdentifierProvider(httpContextAccessor);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage(
                "The user identifier claim is required. (Parameter 'httpContextAccessor')"
            );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        httpContextAccessor.ClearSubstitute();
    }
}
