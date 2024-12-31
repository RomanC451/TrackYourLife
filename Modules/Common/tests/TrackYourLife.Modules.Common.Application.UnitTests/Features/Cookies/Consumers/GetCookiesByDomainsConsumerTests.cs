using MassTransit;
using TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;

namespace TrackYourLife.Modules.Common.Application.UnitTests.Features.Cookies.Consumers;

public sealed class GetCookiesByDomainsConsumerTest : IDisposable
{
    private readonly GetCookiesByDomainsConsumer sut;
    private readonly ICookieQuery cookieQuery = Substitute.For<ICookieQuery>();

    public GetCookiesByDomainsConsumerTest()
    {
        sut = new GetCookiesByDomainsConsumer(cookieQuery);
    }

    [Fact]
    public async Task Consume_WhenDomainsAreValid_ShouldReturnCookies()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var cookies = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name", "value", "/", "example.com")
        };

        var request = new GetCookiesByDomainsRequest(["/"]);
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        cookieQuery.GetCookiesByDomainAsync("/", CancellationToken.None).Returns(cookies);

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies[0].Name.Should().Be("name");
        response.Cookies[0].Value.Should().Be("value");
        response.Cookies[0].Domain.Should().Be("/");
        response.Cookies[0].Path.Should().Be("example.com");

        await cookieQuery.Received(1).GetCookiesByDomainAsync("/", CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenDomainIsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var request = new GetCookiesByDomainsRequest([]);
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().BeEmpty();
    }

    [Fact]
    public async Task Consume_WhenCookiesAreNotFound_ShouldReturnEmptyList()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var request = new GetCookiesByDomainsRequest(new[] { "example.com" });
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        cookieQuery.GetCookiesByDomainAsync("example.com", CancellationToken.None).Returns([]);

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().BeEmpty();
    }

    public void Dispose()
    {
        cookieQuery.ClearSubstitute();
        GC.SuppressFinalize(this);
    }
}
