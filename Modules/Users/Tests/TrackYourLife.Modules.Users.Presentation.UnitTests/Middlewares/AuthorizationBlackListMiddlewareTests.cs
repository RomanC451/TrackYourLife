using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Presentation.Middlewares;
using Xunit;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Middlewares;

public class AuthorizationBlackListMiddlewareTests
{
    private readonly RequestDelegate _next;
    private readonly IAuthorizationBlackListStorage _blackListStorage;
    private readonly AuthorizationBlackListMiddleware _middleware;
    private readonly HttpContext _context;

    public AuthorizationBlackListMiddlewareTests()
    {
        _next = Substitute.For<RequestDelegate>();
        _blackListStorage = Substitute.For<IAuthorizationBlackListStorage>();
        _middleware = new AuthorizationBlackListMiddleware(_next, _blackListStorage);
        _context = new DefaultHttpContext();
    }

    [Fact]
    public async Task InvokeAsync_WhenEndpointIsAnonymous_ShouldCallNext()
    {
        // Arrange
        var endpoint = new Endpoint(
            null,
            new EndpointMetadataCollection(new AllowAnonymousAttribute()),
            "TestEndpoint"
        );
        _context.SetEndpoint(endpoint);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        await _next.Received(1).Invoke(_context);
        _context.Response.StatusCode.Should().NotBe(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WhenTokenIsNotInBlackList_ShouldCallNext()
    {
        // Arrange
        var token = "valid-token";
        _context.Request.Headers.Authorization = $"Bearer {token}";
        _blackListStorage.Contains(token).Returns(false);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        await _next.Received(1).Invoke(_context);
        _context.Response.StatusCode.Should().NotBe(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WhenTokenIsInBlackList_ShouldReturnUnauthorized()
    {
        // Arrange
        var token = "blacklisted-token";
        _context.Request.Headers.Authorization = $"Bearer {token}";
        _blackListStorage.Contains(token).Returns(true);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        await _next.DidNotReceive().Invoke(_context);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoAuthorizationHeader_ShouldCallNext()
    {
        // Arrange
        _context.Request.Headers.Authorization = string.Empty;

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        await _next.Received(1).Invoke(_context);
        _context.Response.StatusCode.Should().NotBe(StatusCodes.Status401Unauthorized);
    }
}
